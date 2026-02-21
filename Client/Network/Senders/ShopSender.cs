using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class ShopSender
{
    public static void ShopBuy(short slot) => Send.Packet(ClientPacket.ShopBuy, new ShopBuyPacket { Slot = slot });

    public static void ShopSell(short slot, short amount) =>
        Send.Packet(ClientPacket.ShopSell, new ShopSellPacket { Slot = slot, Amount = amount });

    public static void ShopClose() => Send.Packet(ClientPacket.ShopClose, new ShopClosePacket());
}
