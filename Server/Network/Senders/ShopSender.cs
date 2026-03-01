using CryBits.Entities.Shop;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class ShopSender
{
    public static void Shops(GameSession session)
    {
        PackageSender.ToPlayer(session, new ShopsPacket { List = Shop.List });
    }

    public static void ShopOpen(Player player, Shop shop)
    {
        PackageSender.ToPlayer(player, new ShopOpenPacket { Id = shop.GetId() });
    }
}
