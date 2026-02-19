using System;
using System.Collections.Generic;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Library.Repositories;
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
        Config = (ServerConfig)data.ReadObject();

        // Salva os dados
        SettingsRepository.Write();
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
        ClassRepository.WriteAll();

        // Envia os novos dados para todos jogadores conectados
        for (int i = 0; i < Account.List.Count; i++)
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
        MapRepository.WriteAll();

        // Envia os novos dados para todos jogadores 
        foreach (var tempMap in TempMap.List.Values)
        {
            // Itens do mapa
            tempMap.SpawnItems();

            // Envia o mapa para todos os jogadores que estão nele
            for (int n = 0; n < Account.List.Count; n++)
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
        NpcRepository.WriteAll();

        // Envia os novos dados para todos jogadores conectados
        for (int i = 0; i < Account.List.Count; i++)
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
        ItemRepository.WriteAll();

        // Envia os novos dados para todos jogadores conectados
        for (int i = 0; i < Account.List.Count; i++)
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
        ShopRepository.WriteAll();

        // Envia os novos dados para todos jogadores conectados
        for (int i = 0; i < Account.List.Count; i++)
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
