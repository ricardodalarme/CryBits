using CryBits.Transport;
using CryBits.Transport.Packets.Server;
using System.Linq.Expressions;
using System.Reflection;

namespace CryBits.Client.Framework.Network;

/// <summary>
/// Type-keyed dispatch table for server-to-client packets.
/// MemoryPack Union deserialization preserves the concrete type, so packet.GetType() is the key.
/// </summary>
public static class PacketDispatcher
{
    private static readonly Dictionary<Type, Action<IServerPacket>> _handlers = [];

    /// <summary>
    /// Discovers all instance <see cref="PacketHandlerAttribute"/> methods on <paramref name="handler"/>
    /// and registers a bound delegate for each.  The instance is captured so that dependencies
    /// injected via the constructor are available when the handler is invoked.
    /// </summary>
    public static void Register(object handler)
    {
        var methods = handler.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<PacketHandlerAttribute>() is not null);

        foreach (var method in methods)
        {
            var packetParam = method.GetParameters()
                .FirstOrDefault(p => typeof(IServerPacket).IsAssignableFrom(p.ParameterType))
                ?? throw new InvalidOperationException(
                    $"[PacketHandler] on '{method.DeclaringType?.Name}.{method.Name}' " +
                    $"requires a parameter implementing IServerPacket.");

            var packetType = packetParam.ParameterType;

            if (_handlers.ContainsKey(packetType))
                throw new InvalidOperationException(
                    $"Duplicate [PacketHandler] for '{packetType.Name}' " +
                    $"on '{method.DeclaringType?.Name}.{method.Name}'.");

            _handlers[packetType] = BuildInstanceHandler(method, handler);
        }
    }

    public static void Dispatch(byte[] data)
    {
        var packet = PacketSerializer.Deserialize<IServerPacket>(data);

        if (_handlers.TryGetValue(packet.GetType(), out var handler))
        {
            try
            {
                handler(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PacketDispatcher] Handler for '{packet.GetType().Name}' threw: {ex}");
            }
        }
    }

    private static Action<IServerPacket> BuildInstanceHandler(MethodInfo method, object instance)
    {
        var packetParam = Expression.Parameter(typeof(IServerPacket), "packet");
        var instanceExpr = Expression.Constant(instance);
        var methodParams = method.GetParameters();

        var packetParamIndex = Array.FindIndex(methodParams,
            p => typeof(IServerPacket).IsAssignableFrom(p.ParameterType));

        Expression callExpr = Expression.Call(instanceExpr, method,
            methodParams.Select((p, i) =>
                i == packetParamIndex
                    ? (Expression)Expression.Convert(packetParam, p.ParameterType)
                    : Expression.Default(p.ParameterType)).ToArray());

        return Expression.Lambda<Action<IServerPacket>>(callExpr, packetParam).Compile();
    }
}
