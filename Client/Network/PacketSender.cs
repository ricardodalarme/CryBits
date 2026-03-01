using CryBits.Extensions;
using CryBits.Packets.Client;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Client.Network;

internal static class PacketSender
{
    public static void Packet(IClientPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        NetworkClient.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }
}
