using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class ShopSender
{
    public static void ShopBuy(short slot) => Send.Packet(new ShopBuyPacket { Slot = slot });

    public static void ShopSell(short slot, short amount) =>
        Send.Packet(new ShopSellPacket { Slot = slot, Amount = amount });

    public static void ShopClose() => Send.Packet(new ShopClosePacket());
}
