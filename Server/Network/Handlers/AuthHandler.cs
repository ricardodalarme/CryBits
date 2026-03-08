using CryBits.Packets.Client;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;
using CryBits.Server.World;

namespace CryBits.Server.Network.Handlers;

internal sealed class AuthHandler(AuthSender authSender, AuthSystem authSystem)
{
    public static AuthHandler Instance { get; } = new(AuthSender.Instance, AuthSystem.Instance);

    private readonly AuthSender _authSender = authSender;
    private readonly AuthSystem _authSystem = authSystem;

    [PacketHandler]
    internal void Latency(GameSession session, LatencyPacket packet)
    {
        _authSender.Latency(session);
    }

    [PacketHandler]
    internal void Connect(GameSession session, ConnectPacket packet)
    {
        _authSystem.Connect(session, packet);
    }

    [PacketHandler]
    internal void Register(GameSession session, RegisterPacket packet)
    {
        _authSystem.Register(session, packet);
    }
}
