using System.Collections.Generic;
using System.IO;
using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Map;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network.Senders;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Entities;
using CryBits.Extensions;
using Entity = Arch.Core.Entity;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class MapHandler(GameContext context, MapSender mapSender, AudioManager audioManager)
{
    [PacketHandler]
    internal void MapRevision(MapRevisionPacket packet)
    {
        var needed = false;
        var id = packet.MapId;
        var currentRevision = packet.Revision;

        // Destroy entities for other players leaving this map (they'll re-spawn on PlayerData)
        var myEntity = context.LocalPlayer.Entity;
        var toDestroy = new List<Entity>();
        var playerQuery = new QueryDescription().WithAll<PlayerTagComponent>();
        context.World.Query(in playerQuery, (Entity e) =>
        {
            if (e != myEntity) toDestroy.Add(e);
        });
        foreach (var e in toDestroy) context.World.Destroy(e);

        // Check whether the map data needs to be downloaded
        if (File.Exists(Directories.MapsData.FullName + id + Directories.Format) ||
            CryBits.Entities.Map.Map.List.ContainsKey(id))
        {
            if (!CryBits.Entities.Map.Map.List.ContainsKey(id))
            {
                CryBits.Entities.Map.Map.List.Add(id, MapRepository.Read(id));
                context.CurrentMap.Data.Update();
            }

            if (CryBits.Entities.Map.Map.List[id].Revision != currentRevision)
                needed = true;
        }
        else
            needed = true;

        // Request map data
        mapSender.RequestMap(needed);
    }

    [PacketHandler]
    internal void Map(MapPacket packet)
    {
        var map = packet.Map;
        var id = map.Id;

        // Store map data
        if (!CryBits.Entities.Map.Map.List.TryAdd(id, map)) CryBits.Entities.Map.Map.List[id] = map;
        else context.Maps.TryAdd(id, new ClientMap(map));

        context.CurrentMap = context.Maps[id];

        // Persist map to disk
        MapRepository.Write(map);

        // Reset weather ECS state for the new map and spawn the fog entity.
        WeatherSpawner.Reset(context.World, context.CurrentMap.Data.Weather.Type);
        FogSpawner.Spawn(context.World, context.CurrentMap.Data.Fog);
        context.CurrentMap.Data.Update();
    }

    [PacketHandler]
    internal void JoinMap(JoinMapPacket _)
    {
        // Play map background music if present
        if (string.IsNullOrEmpty(context.CurrentMap.Data.Music))
            audioManager.StopMusic();
        else
            audioManager.PlayMusic(context.CurrentMap.Data.Music);
    }

    [PacketHandler]
    internal void MapItems(MapItemsPacket packet)
    {
        var world = context.World;

        // Destroy all stale map-item entities instantly (no CommandBuffer needed)
        var query = new QueryDescription().WithAll<GroundItemComponent>();
        world.Destroy(in query);

        // Spawn an ECS entity for every item the server reported.
        foreach (var itemData in packet.Items)
            GroundItemSpawner.Spawn(world, Item.List.Get(itemData.ItemId), itemData.X, itemData.Y);
    }
}
