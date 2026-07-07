using CryBits.Host.Core;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;

namespace CryBits.Host.Network.Handlers;

internal sealed class AckHandler
{
    [PacketHandler]
    internal void Handle(Session session, AckPacket packet)
    {
        if (session.ReplicationState != null && packet.LastReceivedTick > session.ReplicationState.LastAckedTick)
        {
            session.ReplicationState.LastAckedTick = packet.LastReceivedTick;
        }
    }
}
