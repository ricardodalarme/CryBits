using CryBits.Client.UI.Game.Views;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Network;
using CryBits.Network.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ShopHandler
{
    private readonly DefinitionCatalog _catalog;
    public ShopHandler(DefinitionCatalog catalog) => _catalog = catalog;
    [PacketHandler]
    internal void Shops(ShopsPacket packet)
    {
        // Read shops dictionary
        _catalog.Shops = packet.List;
    }

    [PacketHandler]
    internal void ShopOpen(ShopOpenPacket packet)
    {
        // Open shop panel
        var shop = _catalog.Shops.Get(packet.Id);
        if (shop != null) ShopView.Open(shop);
        else ShopView.Panel.Visible = false;
    }
}
