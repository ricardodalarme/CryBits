using System;
using System.Collections.Generic;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Library;
using CryBits.Server.Network.Senders;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal static class EditorHandler
{
    internal static void WriteSettings(Account account, NetDataReader data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Altera os dados
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

        // Salva os dados
        Write.Defaults();
    }

    internal static void WriteClasses(Account account, NetDataReader data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Class.List = (Dictionary<Guid, Class>)data.ReadObject();
        Write.Classes();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                ClassSender.Classes(Account.List[i]);
    }

    internal static void WriteMaps(Account account, NetDataReader data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Map.List = (Dictionary<Guid, Map>)data.ReadObject();
        Write.Maps();

        // Envia os novos dados para todos jogadores 
        foreach (var tempMap in TempMap.List.Values)
        {
            // Itens do mapa
            tempMap.SpawnItems();

            // Envia o mapa para todos os jogadores que estão nele
            for (byte n = 0; n < Account.List.Count; n++)
                if (Account.List[n] != account)
                    if (Account.List[n].Character.Map == tempMap || Account.List[n].InEditor)
                        MapSender.Map(Account.List[n], tempMap.Data);
        }
    }

    internal static void WriteNpcs(Account account, NetDataReader data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Npc.List = (Dictionary<Guid, Npc>)data.ReadObject();
        Write.Npcs();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                NpcSender.Npcs(Account.List[i]);
    }

    internal static void WriteItems(Account account, NetDataReader data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Item.List = (Dictionary<Guid, Item>)data.ReadObject();
        Write.Items();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                ItemSender.Items(Account.List[i]);
    }

    internal static void WriteShops(Account account, NetDataReader data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
        Write.Shops();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                ShopSender.Shops(Account.List[i]);
    }

    internal static void RequestSetting(Account account)
    {
        SettingsSender.ServerData(account);
    }

    internal static void RequestClasses(Account account)
    {
        ClassSender.Classes(account);
    }

    internal static void RequestMap(Account account, NetDataReader data)
    {
        if (account.InEditor)
            MapSender.Map(account, Map.List.Get(new Guid(data.GetString())));
        else
        {
            var player = account.Character;

            // Se necessário enviar as informações do mapa ao jogador
            if (data.GetBool()) MapSender.Map(player.Account, player.Map.Data);

            // Envia a informação aos outros jogadores
            MapSender.MapPlayers(player);

            // Entra no mapa
            player.GettingMap = false;
            PlayerSender.JoinMap(player);
        }
    }

    internal static void RequestMaps(Account account)
    {
        MapSender.Maps(account);
    }

    internal static void RequestNpcs(Account account)
    {
        NpcSender.Npcs(account);
    }

    internal static void RequestItems(Account account)
    {
        ItemSender.Items(account);
    }

    internal static void RequestShops(Account account)
    {
        ShopSender.Shops(account);
    }
}
