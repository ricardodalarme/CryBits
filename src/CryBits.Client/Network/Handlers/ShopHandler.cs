using CryBits.Client.UI.Game.Views;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ShopHandler(DefinitionCatalog catalog, ShopView shopView)
{
    [PacketHandler]
    internal void ShopOpen(ShopOpenPacket packet)
    {
        var shop = catalog.Shops.Get(packet.Id);
        if (shop != null) shopView.Open(shop);
        else shopView.Panel.Visible = false;
    }
}
