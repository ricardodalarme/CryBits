using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class ShopHandler
{
    [PacketHandler]
    internal static void ShopBuy(Player player, ShopBuyPacket packet)
    {
        ShopSystem.Buy(player, packet.Slot);
    }

    [PacketHandler]
    internal static void ShopSell(Player player, ShopSellPacket packet)
    {
        ShopSystem.Sell(player, (byte)packet.Slot, packet.Amount);
    }

    [PacketHandler]
    internal static void ShopClose(Player player, ShopClosePacket _)
    {
        ShopSystem.Leave(player);
    }
}
