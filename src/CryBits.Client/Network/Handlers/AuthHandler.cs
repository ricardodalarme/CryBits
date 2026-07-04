using CryBits.Client.UI;
using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AuthHandler(DefinitionCatalog catalog, UiContext uiContext, MenuScreen menuScreen)
{
    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        uiContext.UISystem?.MessageBoxes.ShowInfoMessageBox("Server", packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        catalog.Classes = [];
        menuScreen.ShowSelectCharacter([]);
    }
}
