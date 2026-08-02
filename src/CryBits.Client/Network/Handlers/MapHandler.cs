using CryBits.Client.Core;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Network.Senders;
using CryBits.Client.Replication;
using CryBits.Client.Spawners;
using CryBits.Definitions.Maps;
using CryBits.Persistence.Repositories;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Client.Network.Handlers;

internal class MapHandler(
    World world,
    ReplicationState replication,
    ContentSender contentSender,
    AudioManager audioManager,
    MapRepository mapRepository)
{
    [PacketHandler]
    internal void MapRevision(MapRevisionPacket packet)
    {
        var id = packet.MapId;

        var myEntity = replication.LocalPlayerEntity;
        if (myEntity.HasValue)
            world.DestroyWhere(entityId => world.Has<PlayerTag>(entityId) && entityId != myEntity.Value);
        else
            world.DestroyWhere(world.Has<PlayerTag>);

        var map = mapRepository.LoadMap(id);
        if (map is not null)
        {
            world.CurrentMap = map;

            if (string.IsNullOrEmpty(map.Music))
                audioManager.StopMusic();
            else
                audioManager.PlayMusic(map.Music);
        }

        contentSender.RequestMap(true);
    }

    [PacketHandler]
    internal void Map(MapPacket packet)
    {
        var map = packet.Map;

        // Preserve chunks that have already arrived via ChunkPayload
        var oldMap = world.CurrentMap;
        if (oldMap?.Id == map.Id)
            foreach (var (coord, chunk) in oldMap.Chunks)
                map.Chunks.TryAdd(coord, chunk);

        world.CurrentMap = map;

        mapRepository.SaveMap(map);

        WeatherSpawner.Reset(world, world.CurrentMap!.DefaultWeather, audioManager);
        FogSpawner.Spawn(world, world.CurrentMap!.DefaultFog);

        if (string.IsNullOrEmpty(world.CurrentMap!.Music))
            audioManager.StopMusic();
        else
            audioManager.PlayMusic(world.CurrentMap!.Music);
    }

    [PacketHandler]
    internal void HandleChunkRevision(ChunkRevisionPacket packet)
    {
        if (packet.Version < 0)
            world.CurrentMap?.Chunks.Remove(new ChunkCoord(packet.ChunkX, packet.ChunkY));
    }

    [PacketHandler]
    internal void ChunkPayload(ChunkPayload packet)
    {
        var map = world.CurrentMap;
        if (map == null) return;

        TileData[,]? tiles = null;
        if (packet.TileData.Length > 0)
            tiles = MemoryPackSerializer.Deserialize<TileData[,]>(packet.TileData);

        var key = (packet.ChunkX, packet.ChunkY);
        if (map.Chunks.TryGetValue(key, out var existingChunk))
            map.Chunks[key] = existingChunk with
            {
                Version = packet.Version,
                Tiles = tiles ?? existingChunk.Tiles,
                WeatherOverride = packet.WeatherOverride ?? existingChunk.WeatherOverride,
                FogOverride = packet.FogOverride ?? existingChunk.FogOverride,
                LightingOverride = packet.LightingOverride ?? existingChunk.LightingOverride
            };
        else
            map.Chunks[key] = new MapChunk(packet.ChunkX, packet.ChunkY, packet.Version, tiles,
                packet.WeatherOverride, packet.FogOverride, packet.LightingOverride);
    }
}
