using CryBits.Definitions.Catalog;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ItemHandler
{
    [PacketHandler]
    internal void Items(ItemsPacket packet)
    {
        // Read items dictionary
        DefinitionCatalog.Items = packet.List;
    }
}
