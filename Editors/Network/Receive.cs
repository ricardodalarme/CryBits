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
    public static void Handle(NetPacketReader data)
    {
        // Manuseia os dados recebidos
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
        // Mostra a mensagem
        MessageBox.Show(data.GetString());
    }

    private static void Connect()
    {
        // Abre a janela principal
        AvaloniaLoginLauncher.HideLogin();
        AvaloniaMapsLauncher.OpenMapsEditor();
    }

    private static void ServerData(NetDataReader data)
    {
        // Lê os dados
        GameName = data.GetString();
        WelcomeMessage = data.GetString();
        Port = data.GetShort();
        MaxPlayers = data.GetByte();
        MaxCharacters = data.GetByte();
        MaxPartyMembers = data.GetByte();
        MaxMapItems = data.GetByte();
        NumPoints = data.GetByte();
        MinNameLength = data.GetByte();
        MaxNameLength = data.GetByte();
        MinPasswordLength = data.GetByte();
        MaxPasswordLength = data.GetByte();
    }

    private static void Classes(NetDataReader data)
    {
        // Recebe os dados
        Class.List = (Dictionary<Guid, Class>)data.ReadObject();
    }

    private static void Map(NetDataReader data)
    {
        var map = (Map)data.ReadObject();
        var id = map.Id;

        // Obtém o dado
        CryBits.Entities.Map.Map.List[id] = map;
    }

    private static void Npcs(NetDataReader data)
    {
        // Recebe os dados
        Npc.List = (Dictionary<Guid, Npc>)data.ReadObject();
    }

    private static void Items(NetDataReader data)
    {
        // Recebe os dados
        Item.List = (Dictionary<Guid, Item>)data.ReadObject();
    }

    private static void Shops(NetDataReader data)
    {
        // Recebe os dados
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
    }
}