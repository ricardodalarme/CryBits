using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class ShopSender
{
    public static void Shops(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Shops);
        data.WriteObject(Shop.List);
        Send.ToPlayer(account, data);
    }

    public static void ShopOpen(Player player, Shop shop)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.ShopOpen);
        data.Put(shop.GetId());
        Send.ToPlayer(player, data);
    }
}
