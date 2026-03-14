using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class AuthSender(PackageSender packageSender)
{
    public static AuthSender Instance { get; } = new(PackageSender.Instance);

    public void Alert(GameSession session, string message, bool disconnect = true)
    {
        packageSender.ToPlayer(session, new AlertPacket { Message = message });

        if (disconnect) session.Connection.Disconnect();
    }

    public void Connect(GameSession session)
    {
        packageSender.ToPlayer(session, new ConnectPacket());
    }
}
