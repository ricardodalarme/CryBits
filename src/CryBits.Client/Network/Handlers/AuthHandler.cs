using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using CryBits.Definitions.Catalog;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AuthHandler
{
    private readonly DefinitionCatalog _catalog;
    public AuthHandler(DefinitionCatalog catalog) => _catalog = catalog;
    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        // Show alert message
        UI.Alert.Show(packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        // Reset client-side character selection state
        SelectCharacterView.CurrentCharacter = 0;
        _catalog.Classes = [];

        // Open character selection panel
        MenuScreen.CloseMenus();
        SelectCharacterView.SelectCharacterPanel.Visible = true;
    }
}
