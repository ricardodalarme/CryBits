using CryBits.Definitions.Catalog;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Forms;
using CryBits.Transport;
using CryBits.Transport.Packets.Server;

namespace CryBits.Editors.Network.Handlers;

internal class EditorHandler
{
    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        MessageBox.Show(packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        LoginWindow.HideWindow();
        EditorMapsWindow.Open();
    }

    [PacketHandler]
    internal void Classes(ClassesPacket packet)
    {
        DefinitionCatalog.Instance.Classes = packet.List;
    }

    [PacketHandler]
    internal void Map(MapPacket packet)
    {
        var map = packet.Map;
        DefinitionCatalog.Instance.Maps[map.Id] = map;
    }

    [PacketHandler]
    internal void Npcs(NpcsPacket packet)
    {
        DefinitionCatalog.Instance.Npcs = packet.List;
    }

    [PacketHandler]
    internal void Items(ItemsPacket packet)
    {
        DefinitionCatalog.Instance.Items = packet.List;
    }

    [PacketHandler]
    internal void Shops(ShopsPacket packet)
    {
        DefinitionCatalog.Instance.Shops = packet.List;
    }
}
