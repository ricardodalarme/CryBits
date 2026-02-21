using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class ShopSender
{
    public static void Shops(Account account)
    {
        Send.ToPlayer(account, ServerPacket.Shops, new ShopsPacket { List = Shop.List });
    }

    public static void ShopOpen(Player player, Shop shop)
    {
        Send.ToPlayer(player, ServerPacket.ShopOpen, new ShopOpenPacket { Id = shop.GetId() });
    }
}
