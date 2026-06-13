using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Simulation.State;
using CryBits.Server.World;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CryBits.Server.Network;

/// <summary>
/// Builds a type-keyed dispatch table from methods decorated with
/// <see cref="PacketHandlerAttribute"/>.
///
/// The packet type is inferred from the IClientPacket parameter of each handler:
///   void Method(GameSession session, TPacket packet)
///   void Method(EntityId   entityId, TPacket packet)
///
/// On receive, BinaryFormatter already embeds full type info, so
/// packet.GetType() is used as the lookup key — no byte prefix needed.
/// </summary>
internal static class PacketDispatcher
{
    private static readonly Dictionary<Type, Action<GameSession, IClientPacket>> _handlers = [];

    internal static int Count => _handlers.Count;

    /// <summary>
    /// Discovers all instance <see cref="PacketHandlerAttribute"/> methods on <paramref name="handler"/>
    /// and registers a bound delegate for each.  The instance is captured so that dependencies
    /// injected via the constructor are available when the handler is invoked.
    /// </summary>
    internal static void Register(object handler)
    {
        var methods = handler.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<PacketHandlerAttribute>() is not null);

        foreach (var method in methods)
        {
            var packetParam = method.GetParameters()
                .FirstOrDefault(p => typeof(IClientPacket).IsAssignableFrom(p.ParameterType))
                ?? throw new InvalidOperationException(
                    $"[PacketHandler] on '{method.DeclaringType?.Name}.{method.Name}' " +
                    $"requires a parameter implementing IClientPacket.");

            var packetType = packetParam.ParameterType;

            if (_handlers.ContainsKey(packetType))
                throw new InvalidOperationException(
                    $"Duplicate [PacketHandler] for '{packetType.Name}' " +
                    $"on '{method.DeclaringType?.Name}.{method.Name}'.");

            _handlers[packetType] = BuildInstanceHandler(method, handler);
        }
    }

    internal static void Dispatch(GameSession session, NetPacketReader data)
    {
        var packet = (IClientPacket)data.ReadObject();

        if (_handlers.TryGetValue(packet.GetType(), out var handler))
            handler(session, packet);
    }

    private static Action<GameSession, IClientPacket> BuildInstanceHandler(MethodInfo method, object instance)
    {
        var sessionParam = Expression.Parameter(typeof(GameSession), "session");
        var packetParam = Expression.Parameter(typeof(IClientPacket), "packet");
        var instanceExpr = Expression.Constant(instance);

        var methodParams = method.GetParameters();
        var firstParamType = methodParams[0].ParameterType;

        // GameSession-based handler
        if (firstParamType == typeof(GameSession))
        {
            var call = Expression.Call(instanceExpr, method, sessionParam,
                Expression.Convert(packetParam, methodParams[1].ParameterType));

            return Expression.Lambda<Action<GameSession, IClientPacket>>(
                call, sessionParam, packetParam).Compile();
        }

        // EntityId-based handler (null-guarded via Nullable<T>.HasValue)
        if (firstParamType == typeof(EntityId))
        {
            var entityIdVar = Expression.Variable(typeof(EntityId), "entityId");
            var characterProp = Expression.Property(sessionParam, nameof(GameSession.Character));
            var hasValue = Expression.Property(characterProp, "HasValue");
            var value = Expression.Property(characterProp, "Value");
            var assign = Expression.Assign(entityIdVar, value);

            var call = Expression.Call(instanceExpr, method, entityIdVar,
                Expression.Convert(packetParam, methodParams[1].ParameterType));

            var block = Expression.Block([entityIdVar], assign, call);
            var body = Expression.IfThen(hasValue, block);

            return Expression.Lambda<Action<GameSession, IClientPacket>>(
                body, sessionParam, packetParam).Compile();
        }

        throw new InvalidOperationException(
            $"Handler '{method.DeclaringType?.Name}.{method.Name}' has an unsupported first parameter type " +
            $"'{firstParamType.Name}'. Expected GameSession or EntityId.");
    }
}
