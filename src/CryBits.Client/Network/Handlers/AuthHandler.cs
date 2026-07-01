using CryBits.Client.UI;
using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using CryBits.Definitions.Catalog;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AuthHandler(DefinitionCatalog catalog)
{
    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        // Show alert message
        IguinaContext.Instance.UISystem?.MessageBoxes.ShowInfoMessageBox("Server", packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        // Reset client-side character selection state
        SelectCharacterView.CurrentCharacter = 0;
        catalog.Classes = [];

        // Open character selection panel
        MenuScreen.Instance.CloseMenus();
        MenuScreen.Instance.SelectCharacterView.SelectCharacterPanel.Visible = true;
    }
}
