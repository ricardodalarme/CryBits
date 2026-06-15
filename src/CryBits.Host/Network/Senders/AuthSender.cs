using CryBits.Transport.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class AuthSender(PackageSender packageSender)
{
    public static AuthSender Instance { get; } = new(PackageSender.Instance);

    public void Alert(Session session, string message, bool disconnect = true)
    {
        packageSender.ToPlayer(session, new AlertPacket { Message = message });

        if (disconnect) WorldHost.Current.Transport.Disconnect(session.Id);
    }

    public void Connect(Session session)
    {
        packageSender.ToPlayer(session, new ConnectPacket());
    }
}
