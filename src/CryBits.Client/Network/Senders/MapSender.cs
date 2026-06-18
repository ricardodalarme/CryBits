using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Network.Senders;

internal class MapSender(PacketSender packetSender)
{
    public static MapSender Instance { get; } = new(PacketSender.Instance);

    public void RequestMap(bool order) =>
        packetSender.Packet(new RequestMapPacket { SendMap = order }, DeliveryChannel.ReliableUnordered);
}
