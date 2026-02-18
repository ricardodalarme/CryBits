using CryBits.Enums;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Senders;

internal static class MapSender
{
    public static void RequestMap(bool order)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.RequestMap);
        data.Put(order);
        Send.Packet(data);
    }
}
