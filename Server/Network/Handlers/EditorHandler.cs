using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Persistence.Repositories;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal static class EditorHandler
{
    [PacketHandler]
    internal static void WriteSettings(Account account, WriteSettingsPacket packet)
    {
        // Ensure caller has editor access.
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Apply received settings.
        Config = packet.Config;

        // Persist settings.
        SettingsRepository.Write();
    }

    [PacketHandler]
    internal static void WriteClasses(Account account, WriteClassesPacket packet)
    {
        // Ensure caller has editor access.
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Receive and persist new classes.
        Class.List = packet.Classes;
        ClassRepository.WriteAll();

        // Broadcast updated classes to other connected accounts.
        for (var i = 0; i < GameWorld.Current.Accounts.Count; i++)
            if (GameWorld.Current.Accounts[i] != account)
                ClassSender.Classes(GameWorld.Current.Accounts[i]);
    }

    [PacketHandler]
    internal static void WriteMaps(Account account, WriteMapsPacket packet)
    {
        // Ensure caller has editor access.
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Receive and persist new maps.
        Map.List = packet.Maps;
        MapRepository.WriteAll();

        // Update runtime map state and broadcast maps to players/editors.
        foreach (var tempMap in GameWorld.Current.Maps.Values)
        {
            // Spawn any static map items.
            tempMap.SpawnItems();

            // Broadcast the map to players who are on it (and editors).
            for (var n = 0; n < GameWorld.Current.Accounts.Count; n++)
                if (GameWorld.Current.Accounts[n] != account)
                    if (GameWorld.Current.Accounts[n].Character.MapInstance == tempMap || GameWorld.Current.Accounts[n].InEditor)
                        MapSender.Map(GameWorld.Current.Accounts[n], tempMap.Data);
        }
    }

    [PacketHandler]
    internal static void WriteNpcs(Account account, WriteNpcsPacket packet)
    {
        // Ensure caller has editor access.
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Receive and persist new NPCs.
        Npc.List = packet.Npcs;
        NpcRepository.WriteAll();

        // Broadcast NPC updates to other connected accounts.
        for (var i = 0; i < GameWorld.Current.Accounts.Count; i++)
            if (GameWorld.Current.Accounts[i] != account)
                NpcSender.Npcs(GameWorld.Current.Accounts[i]);
    }

    [PacketHandler]
    internal static void WriteItems(Account account, WriteItemsPacket packet)
    {
        // Ensure caller has editor access.
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Receive and persist new items.
        Item.List = packet.Items;
        ItemRepository.WriteAll();

        // Broadcast item updates to other connected accounts.
        for (var i = 0; i < GameWorld.Current.Accounts.Count; i++)
            if (GameWorld.Current.Accounts[i] != account)
                ItemSender.Items(GameWorld.Current.Accounts[i]);
    }

    [PacketHandler]
    internal static void WriteShops(Account account, WriteShopsPacket packet)
    {
        // Ensure caller has editor access.
        if (account.Access < Access.Editor)
        {
            AuthSender.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Receive and persist new shops.
        Shop.List = packet.Shops;
        ShopRepository.WriteAll();

        // Broadcast shop updates to other connected accounts.
        for (var i = 0; i < GameWorld.Current.Accounts.Count; i++)
            if (GameWorld.Current.Accounts[i] != account)
                ShopSender.Shops(GameWorld.Current.Accounts[i]);
    }

    [PacketHandler]
    internal static void RequestSetting(Account account, RequestSettingPacket _)
    {
        SettingsSender.ServerData(account);
    }

    [PacketHandler]
    internal static void RequestClasses(Account account, RequestClassesPacket _)
    {
        ClassSender.Classes(account);
    }

    [PacketHandler]
    internal static void RequestMap(Account account, RequestMapPacket packet)
    {
        if (account.InEditor)
            MapSender.Map(account, Map.List.Get(packet.Id));
        else
        {
            var player = account.Character;

            // Send map data to the requesting player if requested.
            if (packet.SendMap) MapSender.Map(player.Account, player.MapInstance.Data);

            // Send player list for the map to nearby clients.
            MapSender.MapPlayers(player);

            // Finish player map load.
            player.GettingMap = false;
            PlayerSender.JoinMap(player);
        }
    }

    [PacketHandler]
    internal static void RequestMaps(Account account, RequestMapsPacket _)
    {
        MapSender.Maps(account);
    }

    [PacketHandler]
    internal static void RequestNpcs(Account account, RequestNpcsPacket _)
    {
        NpcSender.Npcs(account);
    }

    [PacketHandler]
    internal static void RequestItems(Account account, RequestItemsPacket _)
    {
        ItemSender.Items(account);
    }

    [PacketHandler]
    internal static void RequestShops(Account account, RequestShopsPacket _)
    {
        ShopSender.Shops(account);
    }
}
