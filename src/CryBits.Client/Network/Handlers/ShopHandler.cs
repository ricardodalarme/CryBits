using CryBits.Client.UI.Game;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ShopHandler(DefinitionCatalog catalog, GameScreen gameScreen)
{
    [PacketHandler]
    internal void ShopOpen(ShopOpenPacket packet)
    {
        var shop = catalog.Shops.Get(packet.Id);
        if (shop != null) gameScreen.ShopView.Open(shop);
        else gameScreen.ShopView.Panel.Visible = false;
    }
}
