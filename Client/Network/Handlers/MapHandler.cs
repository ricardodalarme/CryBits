using System.IO;
using Arch.Buffer;
using Arch.Core;
using CryBits.Client.Components;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network.Senders;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Entities;
using CryBits.Extensions;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class MapHandler
{
    [PacketHandler]
    internal static void MapRevision(MapRevisionPacket packet)
    {
        var needed = false;
        var id = packet.MapId;
        var currentRevision = packet.Revision;

        // Clear other player entries
        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                Player.List.RemoveAt(i);

        // Check whether the map data needs to be downloaded
        if (File.Exists(Directories.MapsData.FullName + id + Directories.Format) ||
            CryBits.Entities.Map.Map.List.ContainsKey(id))
        {
            if (!CryBits.Entities.Map.Map.List.ContainsKey(id))
            {
                MapRepository.Read(id);
                MapInstance.Current.Weather.Update();
                MapInstance.Current.Data.Update();
            }

            if (CryBits.Entities.Map.Map.List[id].Revision != currentRevision)
                needed = true;
        }
        else
            needed = true;

        // Request map data
        MapSender.RequestMap(needed);
    }

    [PacketHandler]
    internal static void Map(MapPacket packet)
    {
        var map = packet.Map;
        var id = map.Id;

        // Store map data
        if (CryBits.Entities.Map.Map.List.ContainsKey(id)) CryBits.Entities.Map.Map.List[id] = map;
        else
        {
            CryBits.Entities.Map.Map.List.Add(id, map);
            MapInstance.List.Add(id, new MapInstance(map));
        }

        MapInstance.Current = MapInstance.List[id];

        // Persist map to disk
        MapRepository.Write(map);

        // Update weather particles and map state
        MapInstance.Current.Weather.UpdateType();
        MapInstance.Current.Data.Update();
    }

    [PacketHandler]
    internal static void JoinMap(JoinMapPacket _)
    {
        // Play map background music if present
        if (string.IsNullOrEmpty(MapInstance.Current.Data.Music))
            Music.Stop();
        else
            Music.Play(MapInstance.Current.Data.Music);
    }

    [PacketHandler]
    internal static void MapItems(MapItemsPacket packet)
    {
        var world = GameContext.Instance.World;

        // Destroy all stale map-item entities instantly (no CommandBuffer needed)
        var query = new QueryDescription().WithAll<GroundItemComponent>();
        world.Destroy(in query);

        // Spawn an ECS entity for every item the server reported.
        foreach (var itemData in packet.Items)
            GroundItemSpawner.Spawn(world, Item.List.Get(itemData.ItemId), itemData.X, itemData.Y);
    }
}
