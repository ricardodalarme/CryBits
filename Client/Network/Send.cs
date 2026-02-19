using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Client.Network;

internal static class Send
{
    public static void Packet(NetDataWriter data)
    {
        Socket.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }
}
