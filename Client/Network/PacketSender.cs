using CryBits.Extensions;
using CryBits.Packets.Client;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Client.Network;

internal class PacketSender
{
    public static PacketSender Instance { get; } = new();

    public void Packet(IClientPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        NetworkClient.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }
}
