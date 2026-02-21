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
/// Builds a compile-time dispatch table from methods decorated with
/// <see cref="PacketHandlerAttribute"/>, eliminating the switch statement in
/// <see cref="Receive"/>.
///
/// Handler signatures supported (discovered via reflection at startup):
///   void Method(Account account)
///   void Method(Account account, TPacket packet)
///   void Method(Player  player)
///   void Method(Player  player,  TPacket packet)
///
/// Compiled Expression-tree delegates mean zero reflection overhead per packet
/// after <see cref="Register"/> has been called.
/// </summary>
internal static class PacketDispatcher
{
    private static readonly Dictionary<int, Action<Account, IClientPacket>> _handlers = new();

    /// <summary>Call once at server startup to scan and compile all handlers.</summary>
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
                    $"Duplicate [PacketHandler({attr.PacketId})] on '{method.DeclaringType?.Name}.{method.Name}'.");

            _handlers[attr.PacketId] = BuildHandler(method);
        }

        Console.WriteLine($"PacketDispatcher: {_handlers.Count} handlers registered.");
    }

    /// <summary>
    /// Reads the packet id and payload from <paramref name="data"/> then invokes
    /// the registered handler, if any.
    /// </summary>
    internal static void Dispatch(Account account, NetPacketReader data)
    {
        var id = data.GetByte();
        var packet = (IClientPacket)data.ReadObject();

        if (_handlers.TryGetValue(id, out var handler))
            handler(account, packet);
    }

    // ---------------------------------------------------------------------------
    // Expression-tree builder
    // ---------------------------------------------------------------------------

    private static Action<Account, IClientPacket> BuildHandler(MethodInfo method)
    {
        var accountParam = Expression.Parameter(typeof(Account), "account");
        var packetParam = Expression.Parameter(typeof(IClientPacket), "packet");

        var methodParams = method.GetParameters();
        var firstParamType = methodParams[0].ParameterType;

        // ── Account-based handler ───────────────────────────────────────────────
        if (firstParamType == typeof(Account))
        {
            Expression call = methodParams.Length == 1
                ? Expression.Call(method, accountParam)
                : Expression.Call(method, accountParam,
                    Expression.Convert(packetParam, methodParams[1].ParameterType));

            return Expression.Lambda<Action<Account, IClientPacket>>(
                call, accountParam, packetParam).Compile();
        }

        // ── Player-based handler (null-guarded) ─────────────────────────────────
        // Player player = account.Character;
        // if (player != null) Method(player [, (TPacket)packet]);
        var playerVar = Expression.Variable(typeof(Player), "player");
        var assign = Expression.Assign(
            playerVar,
            Expression.Property(accountParam, nameof(Account.Character)));

        Expression callExpr = methodParams.Length == 1
            ? Expression.Call(method, playerVar)
            : Expression.Call(method, playerVar,
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
