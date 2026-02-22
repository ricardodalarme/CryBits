using CryBits.Packets.Client;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;
using CryBits.Server.World;

namespace CryBits.Server.Network.Handlers;

internal static class AuthHandler
{
    [PacketHandler]
    internal static void Latency(GameSession session, LatencyPacket packet)
    {
        AuthSender.Latency(session);
    }

    [PacketHandler]
    internal static void Connect(GameSession session, ConnectPacket packet)
    {
        AuthSystem.Connect(session, packet);
    }

    [PacketHandler]
    internal static void Register(GameSession session, RegisterPacket packet)
    {
        AuthSystem.Register(session, packet);
    }
}
