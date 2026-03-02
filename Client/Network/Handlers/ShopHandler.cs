using CryBits.Client.UI;
using CryBits.Client.UI.Game.Views;
using CryBits.Entities.Shop;
using CryBits.Extensions;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ShopHandler
{
    [PacketHandler]
    internal void Shops(ShopsPacket packet)
    {
        // Read shops dictionary
        Shop.List = packet.List;
    }

    [PacketHandler]
    internal void ShopOpen(ShopOpenPacket packet)
    {
        // Open shop panel
        var shop = Shop.List.Get(packet.Id);
        if (shop != null) ShopView.Open(shop);
        else ShopView.Panel.Visible = false;
    }
}
