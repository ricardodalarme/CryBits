using CryBits.Extensions;
using CryBits.Packets.Client;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Client.Network;

internal static class Send
{
    public static void Packet(IClientPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        Socket.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }
}
