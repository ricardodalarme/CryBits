using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using Lidgren.Network;
using static CryBits.Extensions.NetworkExtensions;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal static class Receive
{
    public static void Handle(NetIncomingMessage data)
    {
        // Manuseia os dados recebidos
        switch ((ServerPacket)data.ReadByte())
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

    private static void Alert(NetBuffer data)
    {
        // Mostra a mensagem
        MessageBox.Show(data.ReadString());
    }

    private static void Connect()
    {
        // Abre a janela principal
        AvaloniaLoginLauncher.HideLogin();
        EditorMaps.Form = new EditorMaps();
    }

    private static void ServerData(NetBuffer data)
    {
        // Lê os dados
        GameName = data.ReadString();
        WelcomeMessage = data.ReadString();
        Port = data.ReadInt16();
        MaxPlayers = data.ReadByte();
        MaxCharacters = data.ReadByte();
        MaxPartyMembers = data.ReadByte();
        MaxMapItems = data.ReadByte();
        NumPoints = data.ReadByte();
        MinNameLength = data.ReadByte();
        MaxNameLength = data.ReadByte();
        MinPasswordLength = data.ReadByte();
        MaxPasswordLength = data.ReadByte();
    }

    private static void Classes(NetBuffer data)
    {
        // Recebe os dados
        Class.List = (Dictionary<Guid, Class>)data.ReadObject();
    }

    private static void Map(NetBuffer data)
    {
        var map = (Map)data.ReadObject();
        var id = map.Id;

        // Obtém o dado
        if (CryBits.Entities.Map.Map.List.ContainsKey(id)) 
            CryBits.Entities.Map.Map.List[id] = map;
        else
            CryBits.Entities.Map.Map.List.Add(id, map);
    }

    private static void Npcs(NetBuffer data)
    {
        // Recebe os dados
        Npc.List = (Dictionary<Guid, Npc>)data.ReadObject();
    }

    private static void Items(NetBuffer data)
    {
        // Recebe os dados
        Item.List = (Dictionary<Guid, Item>)data.ReadObject();
    }

    private static void Shops(NetBuffer data)
    {
        // Recebe os dados
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
    }
}