using CryBits.Server.Entities;
using CryBits.Server.Systems;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Handlers;

internal static class ShopHandler
{
    internal static void ShopBuy(Player player, NetDataReader data)
    {
        ShopSystem.Buy(player, data.GetShort());
    }

    internal static void ShopSell(Player player, NetDataReader data)
    {
        ShopSystem.Sell(player, data.GetByte(), data.GetShort());
    }

    internal static void ShopClose(Player player)
    {
        ShopSystem.Leave(player);
    }
}
