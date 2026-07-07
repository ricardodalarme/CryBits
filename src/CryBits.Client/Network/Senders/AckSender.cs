using CryBits.Client.Core;
using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Network.Senders;

internal sealed class AckSender(Connection connection, GameContext context)
{
    public void SendAck()
    {
        var packet = new AckPacket
        {
            LastReceivedTick = context.LastAppliedServerTick
        };
        connection.SendPacket(packet, DeliveryChannel.ReliableOrdered);
    }
}
