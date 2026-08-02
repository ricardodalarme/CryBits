using CryBits.Client.Framework.Network;
using CryBits.Client.Replication;
using CryBits.Protocol.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal sealed class AckSender(Connection connection, ReplicationState replication)
{
    public void SendAck()
    {
        var packet = new AckPacket { LastReceivedTick = replication.LastAppliedServerTick };
        connection.SendPacket(packet);
    }
}
