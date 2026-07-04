using CryBits.Client.Framework.Audio;
using CryBits.Client.Network.Senders;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using CryBits.Persistence.Repositories;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using MemoryPack;

namespace CryBits.Client.Network.Handlers;

internal class MapHandler(GameContext context, ContentSender contentSender, AudioManager audioManager, MapRepository mapRepository)
{
    [PacketHandler]
    internal void MapRevision(MapRevisionPacket packet)
    {
        var id = packet.MapId;

        var myEntity = context.LocalPlayer.Entity;
        if (myEntity.HasValue)
            context.World.DestroyWhere(s => s.Has<PlayerTag>() && s.Id != myEntity.Value);
        else
            context.World.DestroyWhere(s => s.Has<PlayerTag>());

        var map = mapRepository.LoadMap(id);
        if (map is not null)
        {
            context.CurrentMap = map;

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
        if (context.CurrentMap?.Id == map.Id)
            foreach (var (coord, chunk) in context.CurrentMap.Chunks)
                map.Chunks.TryAdd(coord, chunk);

        context.CurrentMap = map;

        mapRepository.SaveMap(map);

        WeatherSpawner.Reset(context.World, context.CurrentMap.DefaultWeather);
        FogSpawner.Spawn(context.World, context.CurrentMap.DefaultFog);

        if (string.IsNullOrEmpty(context.CurrentMap.Music))
            audioManager.StopMusic();
        else
            audioManager.PlayMusic(context.CurrentMap.Music);
    }

    [PacketHandler]
    internal void HandleChunkRevision(ChunkRevisionPacket packet)
    {
        if (packet.Version < 0)
            context.CurrentMap?.Chunks.Remove(new ChunkCoord(packet.ChunkX, packet.ChunkY));
    }

    [PacketHandler]
    internal void ChunkPayload(ChunkPayload packet)
    {
        var map = context.CurrentMap;
        if (map == null) return;

        TileData[,]? tiles = null;
        if (packet.TileData.Length > 0)
            tiles = MemoryPackSerializer.Deserialize<TileData[,]>(packet.TileData);

        var key = (packet.ChunkX, packet.ChunkY);
        if (map.Chunks.TryGetValue(key, out var existingChunk))
        {
            map.Chunks[key] = existingChunk with
            {
                Version = packet.Version,
                Tiles = tiles ?? existingChunk.Tiles,
                WeatherOverride = packet.WeatherOverride ?? existingChunk.WeatherOverride,
                FogOverride = packet.FogOverride ?? existingChunk.FogOverride,
                LightingOverride = packet.LightingOverride ?? existingChunk.LightingOverride
            };
        }
        else
        {
            map.Chunks[key] = new MapChunk(packet.ChunkX, packet.ChunkY, packet.Version, tiles,
                packet.WeatherOverride, packet.FogOverride, packet.LightingOverride);
        }
    }
}
