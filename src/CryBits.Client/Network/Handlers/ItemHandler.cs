using CryBits.Definitions.Catalog;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ItemHandler
{
    private readonly DefinitionCatalog _catalog;
    public ItemHandler(DefinitionCatalog catalog) => _catalog = catalog;
    [PacketHandler]
    internal void Items(ItemsPacket packet)
    {
        // Read items dictionary
        _catalog.Items = packet.List;
    }
}
