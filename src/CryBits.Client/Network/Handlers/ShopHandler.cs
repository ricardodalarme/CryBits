using CryBits.Client.UI.Game.Views;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ShopHandler(DefinitionCatalog catalog)
{
    [PacketHandler]
    internal void ShopOpen(ShopOpenPacket packet)
    {
        // Open shop panel
        var shop = catalog.Shops.Get(packet.Id);
        if (shop != null) ShopView.Open(shop);
        else ShopView.Panel.Visible = false;
    }
}
