using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Framework.Network;

public class PacketSender
{
    public static PacketSender Instance { get; } = new();

    public void Packet(IClientPacket packet, DeliveryChannel delivery = DeliveryChannel.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);
        Connection.Instance.Send(bytes, delivery);
    }
}
