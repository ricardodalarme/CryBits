using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Editors.Network.Handlers;

internal static class EditorHandler
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
        Class.List = packet.List;
    }

    [PacketHandler]
    internal static void Map(MapPacket packet)
    {
        var map = packet.Map;
        CryBits.Entities.Map.Map.List[map.Id] = map;
    }

    [PacketHandler]
    internal static void Npcs(NpcsPacket packet)
    {
        Npc.List = packet.List;
    }

    [PacketHandler]
    internal static void Items(ItemsPacket packet)
    {
        Item.List = packet.List;
    }

    [PacketHandler]
    internal static void Shops(ShopsPacket packet)
    {
        Shop.List = packet.List;
    }
}
