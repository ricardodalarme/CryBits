using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Spatial;

namespace CryBits.Client.Worlds;

internal class ClientMap(Map data)
{
    public readonly Map Data = data;

    public bool TileBlocked(int x, int y, Direction direction)
    {
        var dir = direction;
        var nextX = dir == Direction.Right ? x + 1 : dir == Direction.Left ? x - 1 : x;
        var nextY = dir == Direction.Down ? y + 1 : dir == Direction.Up ? y - 1 : y;

        return IsTileBlocked(Data, nextX, nextY);
    }

    private static bool IsTileBlocked(Map map, int x, int y)
    {
        var chunkCoord = ChunkGrid.FromPosition(x, y);
        if (!map.Chunks.TryGetValue(chunkCoord, out var chunk))
            return true;
        if (chunk?.Tiles == null)
            return true;
        var localX = x - chunkCoord.X * ChunkGrid.ChunkSize;
        var localY = y - chunkCoord.Y * ChunkGrid.ChunkSize;
        if (localX < 0 || localX >= ChunkGrid.ChunkSize || localY < 0 || localY >= ChunkGrid.ChunkSize)
            return true;
        return chunk.Tiles[localX, localY].IsBlocked;
    }
}
