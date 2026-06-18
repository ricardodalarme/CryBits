using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Network.Senders;

internal class ShopSender(PacketSender packetSender)
{
    public static ShopSender Instance { get; } = new(PacketSender.Instance);

    public void ShopBuy(short slot) =>
        packetSender.Packet(new ShopBuyPacket { Slot = slot }, DeliveryChannel.ReliableUnordered);

    public void ShopSell(short slot, short amount) =>
        packetSender.Packet(new ShopSellPacket { Slot = slot, Amount = amount }, DeliveryChannel.ReliableUnordered);

    public void ShopClose() =>
        packetSender.Packet(new ShopClosePacket(), DeliveryChannel.ReliableUnordered);
}
