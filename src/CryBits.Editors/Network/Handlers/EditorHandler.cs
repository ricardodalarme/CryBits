using CryBits.Definitions.Catalog;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Forms;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Editors.Network.Handlers;

internal class EditorHandler
{
    [PacketHandler]
    internal static void Alert(AlertPacket packet)
    {
        MessageBox.Show(packet.Message);
    }

    [PacketHandler]
    internal static void Connect(ConnectPacket _)
    {
        LoginWindow.HideWindow();
        EditorMapsWindow.Open();
    }

    [PacketHandler]
    internal static void ServerData(ServerDataPacket packet)
    {
        Config = packet.Config;
    }

    [PacketHandler]
    internal static void Classes(ClassesPacket packet)
    {
        DefinitionCatalog.Classes = packet.List;
    }

    [PacketHandler]
    internal static void Map(MapPacket packet)
    {
        var map = packet.Map;
        DefinitionCatalog.Maps[map.Id] = map;
    }

    [PacketHandler]
    internal static void Npcs(NpcsPacket packet)
    {
        DefinitionCatalog.Npcs = packet.List;
    }

    [PacketHandler]
    internal static void Items(ItemsPacket packet)
    {
        DefinitionCatalog.Items = packet.List;
    }

    [PacketHandler]
    internal static void Shops(ShopsPacket packet)
    {
        DefinitionCatalog.Shops = packet.List;
    }
}
