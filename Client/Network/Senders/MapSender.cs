using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class MapSender
{
    public static void RequestMap(bool order) =>
        Send.Packet(new RequestMapPacket { SendMap = order });
}
