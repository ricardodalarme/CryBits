using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Transport;
using CryBits.Transport.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AuthHandler
{
    private readonly DefinitionCatalog _catalog;
    public AuthHandler(DefinitionCatalog catalog) => _catalog = catalog;
    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        MenuEvents.FireAlert(packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        MenuState.CurrentCharacter = 0;
        _catalog.Classes = [];
        MenuEvents.FireConnectSucceeded();
    }
}
