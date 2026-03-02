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
        ShopView.OpenedShop = Shop.List.Get(packet.Id);
        ShopView.Panel.Visible = ShopView.OpenedShop != null;
    }
}
