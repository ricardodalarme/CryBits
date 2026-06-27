using CryBits.Definitions.Maps;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Host.Replication;

public static class ChunkPayloadBuilder
{
    public static ChunkPayload? Build(World world, Guid mapId, short chunkX, short chunkY)
    {
        if (!world.MapDefs.TryGetValue(mapId, out var mapDef)) return null;
        if (!mapDef.Chunks.TryGetValue(new ChunkCoord(chunkX, chunkY), out var mapChunk)) return null;

        var payload = new ChunkPayload
        {
            MapId = mapId,
            ChunkX = chunkX,
            ChunkY = chunkY,
            Version = mapChunk.Version,
            WeatherOverride = mapChunk.WeatherOverride,
            FogOverride = mapChunk.FogOverride,
            LightingOverride = mapChunk.LightingOverride
        };

        if (mapChunk.Tiles != null)
            payload.TileData = MemoryPackSerializer.Serialize(mapChunk.Tiles);

        return payload;
    }
}
