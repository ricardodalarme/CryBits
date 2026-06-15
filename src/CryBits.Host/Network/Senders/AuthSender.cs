using CryBits.Transport.Packets.Server;
using CryBits.Host.Core;
using CryBits.Transport.Abstractions;

namespace CryBits.Host.Network.Senders;

internal sealed class AuthSender(PackageSender packageSender, ITransport transport)
{
    public void Alert(Session session, string message, bool disconnect = true)
    {
        packageSender.ToPlayer(session, new AlertPacket { Message = message });

        if (disconnect) transport.Disconnect(session.Id);
    }

    public void Connect(Session session)
    {
        packageSender.ToPlayer(session, new ConnectPacket());
    }
}
