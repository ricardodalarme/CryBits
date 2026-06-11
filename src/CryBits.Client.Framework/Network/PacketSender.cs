using CryBits.Extensions;
using CryBits.Packets.Client;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Client.Framework.Network;

public class PacketSender(NetworkClient networkClient)
{
    public static PacketSender Instance { get; } = new(NetworkClient.Instance);

    public void Packet(IClientPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        networkClient.ServerPeer?.Send(data, delivery);
    }
}
