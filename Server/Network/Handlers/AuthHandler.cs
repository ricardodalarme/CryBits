using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class AuthHandler
{
    [PacketHandler]
    internal static void Latency(Account account, LatencyPacket packet)
    {
        AuthSender.Latency(account);
    }

    [PacketHandler]
    internal static void Connect(Account account, ConnectPacket packet)
    {
        AuthSystem.Connect(account, packet);
    }

    [PacketHandler]
    internal static void Register(Account account, RegisterPacket packet)
    {
        AuthSystem.Register(account, packet);
    }
}
