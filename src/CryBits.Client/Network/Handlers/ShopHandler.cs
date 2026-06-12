using CryBits.Definitions.Catalog;
using CryBits.Client.UI.Game.Views;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ShopHandler
{
    [PacketHandler]
    internal void Shops(ShopsPacket packet)
    {
        // Read shops dictionary
        DefinitionCatalog.Shops = packet.List;
    }

    [PacketHandler]
    internal void ShopOpen(ShopOpenPacket packet)
    {
        // Open shop panel
        var shop = DefinitionCatalog.Shops.Get(packet.Id);
        if (shop != null) ShopView.Open(shop);
        else ShopView.Panel.Visible = false;
    }
}
