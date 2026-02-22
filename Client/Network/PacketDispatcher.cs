using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CryBits.Extensions;
using CryBits.Packets.Server;
using LiteNetLib;

namespace CryBits.Client.Network;

/// <summary>
/// Type-keyed dispatch table for server-to-client packets.
/// BinaryFormatter embeds the concrete type, so packet.GetType() is the key —
/// no byte prefix in the wire format.
/// </summary>
internal static class PacketDispatcher
{
    private static readonly Dictionary<Type, Action<IServerPacket>> _handlers = new();

    internal static void Register()
    {
        var methods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
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

            _handlers[packetType] = BuildHandler(method);
        }
    }

    internal static void Dispatch(NetPacketReader data)
    {
        var packet = (IServerPacket)data.ReadObject();

        if (_handlers.TryGetValue(packet.GetType(), out var handler))
            handler(packet);
    }

    private static Action<IServerPacket> BuildHandler(MethodInfo method)
    {
        var packetParam = Expression.Parameter(typeof(IServerPacket), "packet");
        var methodParams = method.GetParameters();

        // Find the IServerPacket-typed parameter index
        var packetParamIndex = Array.FindIndex(methodParams,
            p => typeof(IServerPacket).IsAssignableFrom(p.ParameterType));

        Expression callExpr = Expression.Call(method,
            methodParams.Select((p, i) =>
                i == packetParamIndex
                    ? (Expression)Expression.Convert(packetParam, p.ParameterType)
                    : Expression.Default(p.ParameterType)).ToArray());

        return Expression.Lambda<Action<IServerPacket>>(callExpr, packetParam).Compile();
    }
}
