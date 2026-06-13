using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Persistence.Stores;
using CryBits.Host.Network.Senders;
using CryBits.Simulation.Components;
using System;
using System.IO;
using System.Linq;
using CryBits.Host.Core;

namespace CryBits.Host.Services;

internal sealed class EditorService(
    AuthSender authSender,
    ClassSender classSender,
    MapSender mapSender,
    ItemSender itemSender,
    NpcSender npcSender,
    ShopSender shopSender,
    FileContentStore contentStore,
    DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static EditorService Instance { get; } = new(
        AuthSender.Instance,
        ClassSender.Instance,
        MapSender.Instance,
        ItemSender.Instance,
        NpcSender.Instance,
        ShopSender.Instance,
        new FileContentStore(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Data"))),
        DefinitionCatalog.Instance);

    [PacketHandler]
    internal void WriteClasses(Session session, WriteClassesPacket packet)
    {
        if (session.Account!.AccessLevel < Access.Editor)
        {
            authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        _catalog.Classes = packet.Classes;
        contentStore.SaveAll(_catalog.Classes.Values);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t != session))
            classSender.Classes(t);
    }

    [PacketHandler]
    internal void WriteMaps(Session session, WriteMapsPacket packet)
    {
        if (session.Account!.AccessLevel < Access.Editor)
        {
            authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        _catalog.Maps = packet.Maps;
        contentStore.SaveAll(_catalog.Maps.Values);

        foreach (var tempMap in WorldHost.Current.Maps.Values)
        {
            tempMap.SpawnItems(WorldHost.Current.Entities, _catalog);

            foreach (var t in WorldHost.Current.Sessions.Where(t => t != session))
            {
                if (t.InEditor)
                {
                    mapSender.Map(t, tempMap.Data);
                }
                else if (t.Character.HasValue)
                {
                    var otherPos = WorldHost.Current.Entities.Get(t.Character.Value)?.Get<Position>();
                    if (otherPos?.MapId == tempMap.Id)
                        mapSender.Map(t, tempMap.Data);
                }
            }
        }
    }

    [PacketHandler]
    internal void WriteNpcs(Session session, WriteNpcsPacket packet)
    {
        if (session.Account!.AccessLevel < Access.Editor)
        {
            authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        _catalog.Npcs = packet.Npcs;
        contentStore.SaveAll(_catalog.Npcs.Values);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t != session))
            npcSender.Npcs(t);
    }

    [PacketHandler]
    internal void WriteItems(Session session, WriteItemsPacket packet)
    {
        if (session.Account!.AccessLevel < Access.Editor)
        {
            authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        _catalog.Items = packet.Items;
        contentStore.SaveAll(_catalog.Items.Values);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t != session))
            itemSender.Items(t);
    }

    [PacketHandler]
    internal void WriteShops(Session session, WriteShopsPacket packet)
    {
        if (session.Account!.AccessLevel < Access.Editor)
        {
            authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        _catalog.Shops = packet.Shops;
        contentStore.SaveAll(_catalog.Shops.Values);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t != session))
            shopSender.Shops(t);
    }

    [PacketHandler]
    internal void RequestClasses(Session session, RequestClassesPacket _)
    {
        classSender.Classes(session);
    }

    [PacketHandler]
    internal void RequestMap(Session session, RequestMapPacket packet)
    {
        if (session.InEditor)
            mapSender.Map(session, _catalog.Maps.Get(packet.Id));
        else
        {
            var entityId = session.Character!.Value;
            var world = WorldHost.Current;
            var state = world.Entities.Get(entityId)!;
            var pos = state.Get<Position>()!;
            var combat = state.Get<CombatState>()!;
            var mapInstance = world.Maps[pos.MapId];

            if (packet.SendMap) mapSender.Map(session, mapInstance.Data);

            mapSender.MapPlayers(entityId);

            combat.GettingMap = false;
            PlayerSender.Instance.JoinMap(entityId);
        }
    }

    [PacketHandler]
    internal void RequestMaps(Session session, RequestMapsPacket _)
    {
        mapSender.Maps(session);
    }

    [PacketHandler]
    internal void RequestNpcs(Session session, RequestNpcsPacket _)
    {
        npcSender.Npcs(session);
    }

    [PacketHandler]
    internal void RequestItems(Session session, RequestItemsPacket _)
    {
        itemSender.Items(session);
    }

    [PacketHandler]
    internal void RequestShops(Session session, RequestShopsPacket _)
    {
        shopSender.Shops(session);
    }
}
