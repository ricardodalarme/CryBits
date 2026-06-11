using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal sealed class ShopHandler(ShopSystem shopSystem)
{
    public static ShopHandler Instance { get; } = new(ShopSystem.Instance);

    [PacketHandler]
    internal void ShopBuy(Player player, ShopBuyPacket packet)
    {
        shopSystem.Buy(player, packet.Slot);
    }

    [PacketHandler]
    internal void ShopSell(Player player, ShopSellPacket packet)
    {
        shopSystem.Sell(player, (byte)packet.Slot, packet.Amount);
    }

    [PacketHandler]
    internal void ShopClose(Player player, ShopClosePacket _)
    {
        shopSystem.Leave(player);
    }
}
