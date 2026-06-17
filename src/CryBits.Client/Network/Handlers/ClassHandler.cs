using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Transport;
using CryBits.Transport.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ClassHandler
{
    private readonly DefinitionCatalog _catalog;
    public ClassHandler(DefinitionCatalog catalog) => _catalog = catalog;
    [PacketHandler]
    internal void Classes(ClassesPacket packet)
    {
        // Read classes dictionary
        _catalog.Classes = packet.List;
        MenuEvents.FireClassesUpdated();
    }
}
