using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using LiteNetLib;

namespace CryBits.Client.Framework.Network;

public class PacketSender
{
    public static PacketSender Instance { get; } = new();

    public void Packet(IClientPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);
        Connection.Instance.Send(bytes, delivery);
    }
}
