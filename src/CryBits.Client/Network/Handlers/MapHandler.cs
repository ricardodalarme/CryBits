using CryBits.Client.Framework.Audio;
using CryBits.Client.Network.Senders;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Persistence.Repositories;

namespace CryBits.Client.Network.Handlers;

internal class MapHandler(GameContext context, MapSender mapSender, AudioManager audioManager, DefinitionCatalog catalog, ContentRepository contentRepository)
{
    private readonly DefinitionCatalog _catalog = catalog;
    [PacketHandler]
    internal void MapRevision(MapRevisionPacket packet)
    {
        var id = packet.MapId;
        var currentRevision = packet.Revision;

        // Destroy entities for other players leaving this map (they'll re-spawn on PlayerData)
        var myEntity = context.LocalPlayer.Entity;
        if (myEntity.HasValue)
            context.World.DestroyWhere(s => s.Has<PlayerTag>() && s.Id != myEntity.Value);
        else
            context.World.DestroyWhere(s => s.Has<PlayerTag>());

        // Check whether the map data needs to be downloaded
        var map = contentRepository.Load<Map>(id);
        bool needed;
        if (map is not null)
        {
            needed = map.Revision != currentRevision;

            context.CurrentMap = new ClientMap(map, context.World);
            context.CurrentMap.Data.Update();
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
        context.CurrentMap = new ClientMap(map, context.World);

        // Persist map to disk
        contentRepository.Save(map);

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

        // Destroy all stale map-item entities
        world.DestroyWhere(s => s.Has<GroundItem>());

        // Spawn an ECS entity for every item the server reported.
        foreach (var itemData in packet.Items)
        {
            var item = _catalog.Items.Get(itemData.ItemId);
            if (item is not null) GroundItemSpawner.Spawn(world, item, itemData.X, itemData.Y);
        }
    }
}
