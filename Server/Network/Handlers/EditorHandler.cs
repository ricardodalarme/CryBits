using System.Linq;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Client;
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

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            ClassSender.Classes(t);
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

            foreach (var t in GameWorld.Current.Sessions.Where(t => t != session).Where(t => t.Character?.MapInstance == tempMap || t.InEditor))
                MapSender.Map(t, tempMap.Data);
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

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            NpcSender.Npcs(t);
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

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            ItemSender.Items(t);
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

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            ShopSender.Shops(t);
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

            CryBits.Server.ECS.ServerContext.Instance.World.Remove<CryBits.Server.ECS.Components.LoadingMapTag>(player.EntityId);
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
