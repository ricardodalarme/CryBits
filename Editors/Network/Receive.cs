using System;
using System.Collections.Generic;
using CryBits.Editors.AvaloniaUI;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
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
            case ServerPacket.ServerData: ServerData(data); break;
            case ServerPacket.Classes: Classes(data); break;
            case ServerPacket.Map: Map(data); break;
            case ServerPacket.Npcs: Npcs(data); break;
            case ServerPacket.Items: Items(data); break;
            case ServerPacket.Shops: Shops(data); break;
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

    private static void ServerData(NetDataReader data)
    {
        Config = (ServerConfig)data.ReadObject();
    }

    private static void Classes(NetDataReader data)
    {
        Class.List = (Dictionary<Guid, Class>)data.ReadObject();
    }

    private static void Map(NetDataReader data)
    {
        var map = (Map)data.ReadObject();
        var id = map.Id;

        CryBits.Entities.Map.Map.List[id] = map;
    }

    private static void Npcs(NetDataReader data)
    {
        Npc.List = (Dictionary<Guid, Npc>)data.ReadObject();
    }

    private static void Items(NetDataReader data)
    {
        Item.List = (Dictionary<Guid, Item>)data.ReadObject();
    }

    private static void Shops(NetDataReader data)
    {
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
    }
}
