using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ContentHandler(DefinitionCatalog catalog, MenuScreen menuScreen)
{
    [PacketHandler]
    internal void Classes(ClassesPacket packet)
    {
        catalog.Classes = packet.List;
        menuScreen.CreateCharacterView.UpdateClassLabels(catalog);
    }

    [PacketHandler]
    internal void Items(ItemsPacket packet)
    {
        catalog.Items = packet.List;
    }

    [PacketHandler]
    internal void Npcs(NpcsPacket packet)
    {
        catalog.Npcs = packet.List;
    }

    [PacketHandler]
    internal void Shops(ShopsPacket packet)
    {
        catalog.Shops = packet.List;
    }
}
