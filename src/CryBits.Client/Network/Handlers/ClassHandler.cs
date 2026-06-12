using CryBits.Definitions.Catalog;
using CryBits.Client.UI.Menu.Views;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ClassHandler
{
    [PacketHandler]
    internal void Classes(ClassesPacket packet)
    {
        // Read classes dictionary
        DefinitionCatalog.Classes = packet.List;
        CreateCharacterView.UpdateClassLabels();
    }
}
