using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.State;
using System.Linq.Expressions;
using System.Reflection;

namespace CryBits.Host.Network;

internal sealed class PacketDispatcher
{
    private readonly Dictionary<Type, Action<Session, IClientPacket>> _handlers = [];

    internal int Count => _handlers.Count;

    internal void Register(object handler)
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

    internal void Dispatch(Session session, byte[] data)
    {
        var packet = PacketSerializer.Deserialize<IClientPacket>(data);
        var type = packet.GetType();

        if (_handlers.TryGetValue(type, out var handler))
        {
            handler(session, packet);
        }
    }

    private static Action<Session, IClientPacket> BuildInstanceHandler(MethodInfo method, object instance)
    {
        var sessionParam = Expression.Parameter(typeof(Session), "session");
        var packetParam = Expression.Parameter(typeof(IClientPacket), "packet");
        var instanceExpr = Expression.Constant(instance);

        var methodParams = method.GetParameters();
        var firstParamType = methodParams[0].ParameterType;

        if (firstParamType == typeof(Session))
        {
            var call = Expression.Call(instanceExpr, method, sessionParam,
                Expression.Convert(packetParam, methodParams[1].ParameterType));

            return Expression.Lambda<Action<Session, IClientPacket>>(
                call, sessionParam, packetParam).Compile();
        }

        if (firstParamType == typeof(EntityId))
        {
            var entityIdVar = Expression.Variable(typeof(EntityId), "entityId");
            var characterProp = Expression.Property(sessionParam, nameof(Session.Character));
            var hasValue = Expression.Property(characterProp, "HasValue");
            var value = Expression.Property(characterProp, "Value");
            var assign = Expression.Assign(entityIdVar, value);

            var call = Expression.Call(instanceExpr, method, entityIdVar,
                Expression.Convert(packetParam, methodParams[1].ParameterType));

            var block = Expression.Block([entityIdVar], assign, call);
            var body = Expression.IfThen(hasValue, block);

            return Expression.Lambda<Action<Session, IClientPacket>>(
                body, sessionParam, packetParam).Compile();
        }

        throw new InvalidOperationException(
            $"Handler '{method.DeclaringType?.Name}.{method.Name}' has an unsupported first parameter type " +
            $"'{firstParamType.Name}'. Expected Session or EntityId.");
    }
}
