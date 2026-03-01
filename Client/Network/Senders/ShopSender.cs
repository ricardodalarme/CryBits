using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class ShopSender(PacketSender packetSender)
{
    public static ShopSender Instance { get; } = new(PacketSender.Instance);

    public void ShopBuy(short slot) => packetSender.Packet(new ShopBuyPacket { Slot = slot });

    public void ShopSell(short slot, short amount) =>
        packetSender.Packet(new ShopSellPacket { Slot = slot, Amount = amount });

    public void ShopClose() => packetSender.Packet(new ShopClosePacket());
}
