using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class ShopHandler
{
    [PacketHandler(ClientPacket.ShopBuy)]
    internal static void ShopBuy(Player player, ShopBuyPacket packet)
    {
        ShopSystem.Buy(player, packet.Slot);
    }

    [PacketHandler(ClientPacket.ShopSell)]
    internal static void ShopSell(Player player, ShopSellPacket packet)
    {
        ShopSystem.Sell(player, (byte)packet.Slot, packet.Amount);
    }

    [PacketHandler(ClientPacket.ShopClose)]
    internal static void ShopClose(Player player)
    {
        ShopSystem.Leave(player);
    }
}
