using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Host.Core;
using CryBits.Host.Network.Senders;
using CryBits.Persistence.Repositories;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Simulation.Components;

namespace CryBits.Host.Services;

internal sealed class ContentService(
    AuthSender authSender,
    ContentSender contentSender,
    ContentRepository contentRepository,
    MapRepository mapRepository,
    DefinitionCatalog catalog,
    WorldHost host,
    WorldInitializer worldInitializer)
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
            contentSender.Classes(t, catalog.Classes);
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
        mapRepository.SaveAllMaps(catalog.Maps.Values);
        worldInitializer.Initialize();

        foreach (var tempMap in host.Maps.Values)
            foreach (var t in host.Sessions.Where(t => t != session))
                if (t.InEditor)
                {
                    contentSender.Map(t, tempMap, true);
                }
                else if (t.Character.HasValue)
                {
                    var otherPos = host.Entities.Get<Position>(t.Character.Value);
                    if (otherPos?.MapId == tempMap.Id)
                        contentSender.Map(t, tempMap);
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
            contentSender.Npcs(t, catalog.Npcs);
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
            contentSender.Items(t, catalog.Items);
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
            contentSender.Shops(t, catalog.Shops);
    }

    [PacketHandler]
    internal void RequestClasses(Session session, RequestClassesPacket _)
    {
        contentSender.Classes(session, catalog.Classes);
    }

    [PacketHandler]
    internal void RequestMap(Session session, RequestMapPacket packet)
    {
        if (session.InEditor)
        {
            var map = catalog.Maps.Get(packet.Id);
            if (map is not null) contentSender.Map(session, map, true);
        }
        else
        {
            var entityId = session.Character!.Value;
            var pos = host.Entities.Get<Position>(entityId)!;
            var mapInstance = host.Maps[pos.MapId];

            if (packet.SendMap) contentSender.Map(session, mapInstance);

            host.Simulation.Remove<MapLoadingTag>(entityId);
        }
    }

    [PacketHandler]
    internal void RequestMaps(Session session, RequestMapsPacket _)
    {
        contentSender.Maps(session, catalog.Maps, session.InEditor);
    }

    [PacketHandler]
    internal void RequestNpcs(Session session, RequestNpcsPacket _)
    {
        contentSender.Npcs(session, catalog.Npcs);
    }

    [PacketHandler]
    internal void RequestItems(Session session, RequestItemsPacket _)
    {
        contentSender.Items(session, catalog.Items);
    }

    [PacketHandler]
    internal void RequestShops(Session session, RequestShopsPacket _)
    {
        contentSender.Shops(session, catalog.Shops);
    }
}
