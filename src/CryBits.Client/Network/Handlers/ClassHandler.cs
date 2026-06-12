using CryBits.Definitions.Catalog;
using CryBits.Client.UI.Menu.Views;
using CryBits.Packets.Server;

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
        CreateCharacterView.UpdateClassLabels(_catalog);
    }
}
