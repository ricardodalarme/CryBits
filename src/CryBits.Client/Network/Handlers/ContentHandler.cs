using CryBits.Client.UI.Menu.Views;
using CryBits.Definitions.Catalog;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ContentHandler(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;

    [PacketHandler]
    internal void Classes(ClassesPacket packet)
    {
        _catalog.Classes = packet.List;
        CreateCharacterView.UpdateClassLabels(_catalog);
    }

    [PacketHandler]
    internal void Items(ItemsPacket packet)
    {
        _catalog.Items = packet.List;
    }

    [PacketHandler]
    internal void Npcs(NpcsPacket packet)
    {
        _catalog.Npcs = packet.List;
    }

    [PacketHandler]
    internal void Shops(ShopsPacket packet)
    {
        _catalog.Shops = packet.List;
    }
}
