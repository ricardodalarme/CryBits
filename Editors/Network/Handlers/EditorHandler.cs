using CryBits.Editors.AvaloniaUI;
using CryBits.Entities;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Editors.Network.Handlers;

internal static class EditorHandler
{
    [PacketHandler(ServerPacket.Alert)]
    internal static void Alert(AlertPacket packet)
    {
        MessageBox.Show(packet.Message);
    }

    [PacketHandler(ServerPacket.Connect)]
    internal static void Connect(ConnectPacket _)
    {
        AvaloniaLoginLauncher.HideLogin();
        AvaloniaMapsLauncher.OpenMapsEditor();
    }

    [PacketHandler(ServerPacket.ServerData)]
    internal static void ServerData(ServerDataPacket packet)
    {
        Config = packet.Config;
    }

    [PacketHandler(ServerPacket.Classes)]
    internal static void Classes(ClassesPacket packet)
    {
        Class.List = packet.List;
    }

    [PacketHandler(ServerPacket.Map)]
    internal static void Map(MapPacket packet)
    {
        var map = packet.Map;
        CryBits.Entities.Map.Map.List[map.Id] = map;
    }

    [PacketHandler(ServerPacket.Npcs)]
    internal static void Npcs(NpcsPacket packet)
    {
        Npc.List = packet.List;
    }

    [PacketHandler(ServerPacket.Items)]
    internal static void Items(ItemsPacket packet)
    {
        Item.List = packet.List;
    }

    [PacketHandler(ServerPacket.Shops)]
    internal static void Shops(ShopsPacket packet)
    {
        Shop.List = packet.List;
    }
}
