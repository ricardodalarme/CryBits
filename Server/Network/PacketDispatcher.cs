using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CryBits.Extensions;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
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
internal static class PacketDispatcher
{
    private static readonly Dictionary<Type, Action<Account, IClientPacket>> _handlers = new();

    internal static void Register()
    {
        var methods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
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

    internal static void Dispatch(Account account, NetPacketReader data)
    {
        var packet = (IClientPacket)data.ReadObject();

        if (_handlers.TryGetValue(packet.GetType(), out var handler))
            handler(account, packet);
    }

    private static Action<Account, IClientPacket> BuildHandler(MethodInfo method)
    {
        var accountParam = Expression.Parameter(typeof(Account), "account");
        var packetParam = Expression.Parameter(typeof(IClientPacket), "packet");

        var methodParams = method.GetParameters();
        var firstParamType = methodParams[0].ParameterType;

        // Account-based handler
        if (firstParamType == typeof(Account))
        {
            var call = Expression.Call(method, accountParam,
                Expression.Convert(packetParam, methodParams[1].ParameterType));

            return Expression.Lambda<Action<Account, IClientPacket>>(
                call, accountParam, packetParam).Compile();
        }

        // Player-based handler (null-guarded)
        var playerVar = Expression.Variable(typeof(Player), "player");
        var assign = Expression.Assign(
            playerVar,
            Expression.Property(accountParam, nameof(Account.Character)));

        var callExpr = Expression.Call(method, playerVar,
            Expression.Convert(packetParam, methodParams[1].ParameterType));

        var body = Expression.Block(
            variables: new[] { playerVar },
            assign,
            Expression.IfThen(
                Expression.NotEqual(playerVar, Expression.Constant(null, typeof(Player))),
                callExpr));

        return Expression.Lambda<Action<Account, IClientPacket>>(
            body, accountParam, packetParam).Compile();
    }
}
