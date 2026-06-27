using CryBits.Client.Framework.Audio;
using CryBits.Client.Network.Senders;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using CryBits.Persistence.Repositories;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;

namespace CryBits.Client.Network.Handlers;

internal class MapHandler(GameContext context, ContentSender contentSender, AudioManager audioManager, ContentRepository contentRepository)
{
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

            // Play map background music from cached data
            if (string.IsNullOrEmpty(map.Music))
                audioManager.StopMusic();
            else
                audioManager.PlayMusic(map.Music);
        }
        else
            needed = true;

        // Request map data
        contentSender.RequestMap(needed);
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

        // Play map background music
        if (string.IsNullOrEmpty(context.CurrentMap.Data.Music))
            audioManager.StopMusic();
        else
            audioManager.PlayMusic(context.CurrentMap.Data.Music);
    }
}
