using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Network.Senders;
using CryBits.Simulation.Components;
using CryBits.Host.Core;
using CryBits.Persistence.Repositories;

namespace CryBits.Host.Services;

internal sealed class EditorService(
    AuthSender authSender,
    ClassSender classSender,
    MapSender mapSender,
    ItemSender itemSender,
    NpcSender npcSender,
    ShopSender shopSender,
    PlayerSender playerSender,
    ContentRepository contentRepository,
    DefinitionCatalog catalog,
    WorldHost host)
{
    [PacketHandler]
    internal void WriteClasses(Session session, WriteClassesPacket packet)
    {
        if (session.Account!.AccessLevel < Access.Editor)
        {
            authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        catalog.Classes = packet.Classes;
        contentRepository.SaveAll(catalog.Classes.Values);

        foreach (var t in host.Sessions.Where(t => t != session))
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

        catalog.Maps = packet.Maps;
        contentRepository.SaveAll(catalog.Maps.Values);

        foreach (var tempMap in host.Maps.Values)
        {
            tempMap.SpawnItems(host.Entities);

            foreach (var t in host.Sessions.Where(t => t != session))
            {
                if (t.InEditor)
                {
                    mapSender.Map(t, tempMap.Data);
                }
                else if (t.Character.HasValue)
                {
                    var otherPos = host.Entities.Get(t.Character.Value)?.Get<Position>();
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

        catalog.Npcs = packet.Npcs;
        contentRepository.SaveAll(catalog.Npcs.Values);

        foreach (var t in host.Sessions.Where(t => t != session))
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

        catalog.Items = packet.Items;
        contentRepository.SaveAll(catalog.Items.Values);

        foreach (var t in host.Sessions.Where(t => t != session))
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

        catalog.Shops = packet.Shops;
        contentRepository.SaveAll(catalog.Shops.Values);

        foreach (var t in host.Sessions.Where(t => t != session))
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
            mapSender.Map(session, catalog.Maps.Get(packet.Id));
        else
        {
            var entityId = session.Character!.Value;
            var state = host.Entities.Get(entityId)!;
            var pos = state.Get<Position>()!;
            var mapInstance = host.Maps[pos.MapId];

            if (packet.SendMap) mapSender.Map(session, mapInstance.Data);

            mapSender.MapPlayers(entityId);

            pos.LoadingMap = false;
            playerSender.JoinMap(entityId);
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
