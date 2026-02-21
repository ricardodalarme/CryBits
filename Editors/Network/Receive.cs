using CryBits.Editors.AvaloniaUI;
using CryBits.Entities;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Packets.Server;
using LiteNetLib;
using LiteNetLib.Utils;
using static CryBits.Extensions.NetworkExtensions;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal static class Receive
{
    /// <summary>Process incoming editor network packets.</summary>
    public static void Handle(NetPacketReader data)
    {
        switch ((ServerPacket)data.GetByte())
        {
            case ServerPacket.Alert: Alert(data); break;
            case ServerPacket.Connect: Connect(); break;
            case ServerPacket.ServerData: ServerData((ServerDataPacket)data.ReadObject()); break;
            case ServerPacket.Classes: Classes((ClassesPacket)data.ReadObject()); break;
            case ServerPacket.Map: Map((MapPacket)data.ReadObject()); break;
            case ServerPacket.Npcs: Npcs((NpcsPacket)data.ReadObject()); break;
            case ServerPacket.Items: Items((ItemsPacket)data.ReadObject()); break;
            case ServerPacket.Shops: Shops((ShopsPacket)data.ReadObject()); break;
        }
    }

    private static void Alert(NetDataReader data)
    {
        MessageBox.Show(data.GetString());
    }

    private static void Connect()
    {
        AvaloniaLoginLauncher.HideLogin();
        AvaloniaMapsLauncher.OpenMapsEditor();
    }

    private static void ServerData(ServerDataPacket packet)
    {
        Config = packet.Config;
    }

    private static void Classes(ClassesPacket packet)
    {
        Class.List = packet.List;
    }

    private static void Map(MapPacket packet)
    {
        var map = packet.Map;
        var id = map.Id;

        CryBits.Entities.Map.Map.List[id] = map;
    }

    private static void Npcs(NpcsPacket packet)
    {
        Npc.List = packet.List;
    }

    private static void Items(ItemsPacket packet)
    {
        Item.List = packet.List;
    }

    private static void Shops(ShopsPacket packet)
    {
        Shop.List = packet.List;
    }
}
