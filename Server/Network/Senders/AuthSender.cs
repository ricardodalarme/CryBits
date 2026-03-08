using CryBits.Packets.Server;
using CryBits.Server.Network;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class AuthSender(PackageSender packageSender)
{
    public static AuthSender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void Alert(GameSession session, string message, bool disconnect = true)
    {
        _packageSender.ToPlayer(session, new AlertPacket { Message = message });

        if (disconnect) session.Connection.Disconnect();
    }

    public void Latency(GameSession session)
    {
        _packageSender.ToPlayer(session, new LatencyPacket());
    }

    public void Connect(GameSession session)
    {
        _packageSender.ToPlayer(session, new ConnectPacket());
    }
}
