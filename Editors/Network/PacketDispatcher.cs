using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using LiteNetLib;

namespace CryBits.Editors.Network;

/// <summary>
/// Builds a compile-time dispatch table from methods decorated with
/// <see cref="PacketHandlerAttribute"/>, eliminating the switch statement in Receive.
/// </summary>
internal static class PacketDispatcher
{
    private static readonly Dictionary<int, Action<IServerPacket>> _handlers = new();

    /// <summary>Call once at startup to scan and compile all handlers.</summary>
    internal static void Register()
    {
        var methods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            .Where(m => m.GetCustomAttribute<PacketHandlerAttribute>() is not null);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<PacketHandlerAttribute>()!;

            if (_handlers.ContainsKey(attr.PacketId))
                throw new InvalidOperationException(
                    $"Duplicate [PacketHandler] for packet id {attr.PacketId} " +
                    $"on '{method.DeclaringType?.Name}.{method.Name}'.");

            _handlers[attr.PacketId] = BuildHandler(method);
        }
    }

    internal static void Dispatch(NetPacketReader data)
    {
        var id = (int)(ServerPacket)data.GetByte();
        var packet = (IServerPacket)data.ReadObject();

        if (_handlers.TryGetValue(id, out var handler))
            handler(packet);
    }

    private static Action<IServerPacket> BuildHandler(MethodInfo method)
    {
        var packetParam = Expression.Parameter(typeof(IServerPacket), "packet");

        var methodParams = method.GetParameters();

        Expression callExpr = methodParams.Length == 0
            ? Expression.Call(method)
            : Expression.Call(method,
                Expression.Convert(packetParam, methodParams[0].ParameterType));

        return Expression.Lambda<Action<IServerPacket>>(callExpr, packetParam).Compile();
    }
}
