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
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal static class EditorHandler
{
    [PacketHandler(ClientPacket.WriteSettings)]
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

    [PacketHandler(ClientPacket.WriteClasses)]
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
        for (var i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                ClassSender.Classes(Account.List[i]);
    }

    [PacketHandler(ClientPacket.WriteMaps)]
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
        foreach (var tempMap in TempMap.List.Values)
        {
            // Spawn any static map items.
            tempMap.SpawnItems();

            // Broadcast the map to players who are on it (and editors).
            for (var n = 0; n < Account.List.Count; n++)
                if (Account.List[n] != account)
                    if (Account.List[n].Character.Map == tempMap || Account.List[n].InEditor)
                        MapSender.Map(Account.List[n], tempMap.Data);
        }
    }

    [PacketHandler(ClientPacket.WriteNpcs)]
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
        for (var i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                NpcSender.Npcs(Account.List[i]);
    }

    [PacketHandler(ClientPacket.WriteItems)]
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
        for (var i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                ItemSender.Items(Account.List[i]);
    }

    [PacketHandler(ClientPacket.WriteShops)]
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
        for (var i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                ShopSender.Shops(Account.List[i]);
    }

    [PacketHandler(ClientPacket.RequestSetting)]
    internal static void RequestSetting(Account account)
    {
        SettingsSender.ServerData(account);
    }

    [PacketHandler(ClientPacket.RequestClasses)]
    internal static void RequestClasses(Account account)
    {
        ClassSender.Classes(account);
    }

    [PacketHandler(ClientPacket.RequestMap)]
    internal static void RequestMap(Account account, RequestMapPacket packet)
    {
        if (account.InEditor)
            MapSender.Map(account, Map.List.Get(packet.Id));
        else
        {
            var player = account.Character;

            // Send map data to the requesting player if requested.
            if (packet.SendMap) MapSender.Map(player.Account, player.Map.Data);

            // Send player list for the map to nearby clients.
            MapSender.MapPlayers(player);

            // Finish player map load.
            player.GettingMap = false;
            PlayerSender.JoinMap(player);
        }
    }

    [PacketHandler(ClientPacket.RequestMaps)]
    internal static void RequestMaps(Account account)
    {
        MapSender.Maps(account);
    }

    [PacketHandler(ClientPacket.RequestNpcs)]
    internal static void RequestNpcs(Account account)
    {
        NpcSender.Npcs(account);
    }

    [PacketHandler(ClientPacket.RequestItems)]
    internal static void RequestItems(Account account)
    {
        ItemSender.Items(account);
    }

    [PacketHandler(ClientPacket.RequestShops)]
    internal static void RequestShops(Account account)
    {
        ShopSender.Shops(account);
    }
}
