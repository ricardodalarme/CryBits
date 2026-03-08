using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CryBits.Extensions;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.World;
using LiteNetLib;

namespace CryBits.Server.Network;

/// <summary>
/// Builds a type-keyed dispatch table from methods decorated with
/// <see cref="PacketHandlerAttribute"/>.
///
/// The packet type is inferred from the IClientPacket parameter of each handler:
///   void Method(Account account, TPacket packet)
///   void Method(Player  player,  TPacket packet)
///
/// On receive, BinaryFormatter already embeds full type info, so
/// packet.GetType() is used as the lookup key — no byte prefix needed.
/// </summary>
internal sealed class PacketDispatcher
{
    public static PacketDispatcher Instance { get; } = new();

    private readonly Dictionary<Type, Action<GameSession, IClientPacket>> _handlers = new();

    internal void Register()
    {
        var methods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
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

            _handlers[packetType] = BuildHandler(method);
        }

        Console.WriteLine($"PacketDispatcher: {_handlers.Count} handlers registered.");
    }

    internal void Dispatch(GameSession session, NetPacketReader data)
    {
        var packet = (IClientPacket)data.ReadObject();

        if (_handlers.TryGetValue(packet.GetType(), out var handler))
            handler(session, packet);
    }

    private static Action<GameSession, IClientPacket> BuildHandler(MethodInfo method)
    {
        var sessionParam = Expression.Parameter(typeof(GameSession), "session");
        var packetParam = Expression.Parameter(typeof(IClientPacket), "packet");

        var methodParams = method.GetParameters();
        var firstParamType = methodParams[0].ParameterType;

        // For instance methods, resolve the handler instance via the declaring type's Instance property.
        Expression target = null;
        if (!method.IsStatic)
        {
            var instanceProp = method.DeclaringType!.GetProperty("Instance",
                BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException(
                    $"Instance handler '{method.DeclaringType.Name}.{method.Name}' requires " +
                    $"a public static 'Instance' property on '{method.DeclaringType.Name}'.");
            target = Expression.Constant(instanceProp.GetValue(null), method.DeclaringType);
        }

        // GameSession-based handler
        if (firstParamType == typeof(GameSession))
        {
            var call = method.IsStatic
                ? Expression.Call(method, sessionParam,
                    Expression.Convert(packetParam, methodParams[1].ParameterType))
                : Expression.Call(target, method, sessionParam,
                    Expression.Convert(packetParam, methodParams[1].ParameterType));

            return Expression.Lambda<Action<GameSession, IClientPacket>>(
                call, sessionParam, packetParam).Compile();
        }

        // Player-based handler (null-guarded)
        var playerVar = Expression.Variable(typeof(Player), "player");
        var assign = Expression.Assign(
            playerVar,
            Expression.Property(sessionParam, nameof(GameSession.Character)));

        var callExpr = method.IsStatic
            ? Expression.Call(method, playerVar,
                Expression.Convert(packetParam, methodParams[1].ParameterType))
            : Expression.Call(target, method, playerVar,
                Expression.Convert(packetParam, methodParams[1].ParameterType));

        var body = Expression.Block(
            variables: [playerVar],
            assign,
            Expression.IfThen(
                Expression.NotEqual(playerVar, Expression.Constant(null, typeof(Player))),
                callExpr));

        return Expression.Lambda<Action<GameSession, IClientPacket>>(
            body, sessionParam, packetParam).Compile();
    }
}
