using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class ShopHandler
{
    internal static void ShopBuy(Player player, ShopBuyPacket packet)
    {
        ShopSystem.Buy(player, packet.Slot);
    }

    internal static void ShopSell(Player player, ShopSellPacket packet)
    {
        ShopSystem.Sell(player, (byte)packet.Slot, packet.Amount);
    }

    internal static void ShopClose(Player player)
    {
        ShopSystem.Leave(player);
    }
}
