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
    internal static void WriteSettings(GameSession session, WriteSettingsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            AuthSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        // Apply received settings.
        Config = packet.Config;

        // Persist settings.
        SettingsRepository.Write();
    }

    [PacketHandler]
    internal static void WriteClasses(GameSession session, WriteClassesPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            AuthSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Class.List = packet.Classes;
        ClassRepository.WriteAll();

        for (var i = 0; i < GameWorld.Current.Sessions.Count; i++)
            if (GameWorld.Current.Sessions[i] != session)
                ClassSender.Classes(GameWorld.Current.Sessions[i]);
    }

    [PacketHandler]
    internal static void WriteMaps(GameSession session, WriteMapsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            AuthSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Map.List = packet.Maps;
        MapRepository.WriteAll();

        foreach (var tempMap in GameWorld.Current.Maps.Values)
        {
            tempMap.SpawnItems();

            for (var n = 0; n < GameWorld.Current.Sessions.Count; n++)
                if (GameWorld.Current.Sessions[n] != session)
                    if (GameWorld.Current.Sessions[n].Character?.MapInstance == tempMap || GameWorld.Current.Sessions[n].InEditor)
                        MapSender.Map(GameWorld.Current.Sessions[n], tempMap.Data);
        }
    }

    [PacketHandler]
    internal static void WriteNpcs(GameSession session, WriteNpcsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            AuthSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Npc.List = packet.Npcs;
        NpcRepository.WriteAll();

        for (var i = 0; i < GameWorld.Current.Sessions.Count; i++)
            if (GameWorld.Current.Sessions[i] != session)
                NpcSender.Npcs(GameWorld.Current.Sessions[i]);
    }

    [PacketHandler]
    internal static void WriteItems(GameSession session, WriteItemsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            AuthSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Item.List = packet.Items;
        ItemRepository.WriteAll();

        for (var i = 0; i < GameWorld.Current.Sessions.Count; i++)
            if (GameWorld.Current.Sessions[i] != session)
                ItemSender.Items(GameWorld.Current.Sessions[i]);
    }

    [PacketHandler]
    internal static void WriteShops(GameSession session, WriteShopsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            AuthSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Shop.List = packet.Shops;
        ShopRepository.WriteAll();

        for (var i = 0; i < GameWorld.Current.Sessions.Count; i++)
            if (GameWorld.Current.Sessions[i] != session)
                ShopSender.Shops(GameWorld.Current.Sessions[i]);
    }

    [PacketHandler]
    internal static void RequestSetting(GameSession session, RequestSettingPacket _)
    {
        SettingsSender.ServerData(session);
    }

    [PacketHandler]
    internal static void RequestClasses(GameSession session, RequestClassesPacket _)
    {
        ClassSender.Classes(session);
    }

    [PacketHandler]
    internal static void RequestMap(GameSession session, RequestMapPacket packet)
    {
        if (session.InEditor)
            MapSender.Map(session, Map.List.Get(packet.Id));
        else
        {
            var player = session.Character;

            if (packet.SendMap) MapSender.Map(player.Session, player.MapInstance.Data);

            MapSender.MapPlayers(player);

            player.GettingMap = false;
            PlayerSender.JoinMap(player);
        }
    }

    [PacketHandler]
    internal static void RequestMaps(GameSession session, RequestMapsPacket _)
    {
        MapSender.Maps(session);
    }

    [PacketHandler]
    internal static void RequestNpcs(GameSession session, RequestNpcsPacket _)
    {
        NpcSender.Npcs(session);
    }

    [PacketHandler]
    internal static void RequestItems(GameSession session, RequestItemsPacket _)
    {
        ItemSender.Items(session);
    }

    [PacketHandler]
    internal static void RequestShops(GameSession session, RequestShopsPacket _)
    {
        ShopSender.Shops(session);
    }
}
