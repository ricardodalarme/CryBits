using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Spatial;

public sealed class ChunkGrid
{
    private readonly Dictionary<ChunkCoord, HashSet<EntityId>> _chunks = [];
    private readonly Dictionary<EntityId, ChunkCoord> _entityChunk = [];

    public const int ChunkSize = 32;

    public static ChunkCoord FromPosition(int tileX, int tileY) =>
        new(
            (short)(tileX >= 0 ? tileX / ChunkSize : (tileX - ChunkSize + 1) / ChunkSize),
            (short)(tileY >= 0 ? tileY / ChunkSize : (tileY - ChunkSize + 1) / ChunkSize));

    public void Add(EntityId entity, int tileX, int tileY)
    {
        var chunk = FromPosition(tileX, tileY);
        if (!_chunks.ContainsKey(chunk))
            _chunks[chunk] = [];
        _chunks[chunk].Add(entity);
        _entityChunk[entity] = chunk;
    }

    public void Move(EntityId entity, int oldX, int oldY, int newX, int newY)
    {
        var oldChunk = FromPosition(oldX, oldY);
        var newChunk = FromPosition(newX, newY);
        if (oldChunk == newChunk) return;
        RemoveFromChunk(entity, oldChunk);
        AddToChunk(entity, newChunk);
        _entityChunk[entity] = newChunk;
    }

    public void Remove(EntityId entity)
    {
        if (_entityChunk.TryGetValue(entity, out var chunk))
        {
            RemoveFromChunk(entity, chunk);
            _entityChunk.Remove(entity);
        }
    }

    public void Remove(EntityId entity, int tileX, int tileY) =>
        Remove(entity);

    public IEnumerable<EntityId> GetEntities(ChunkCoord chunk) =>
        _chunks.TryGetValue(chunk, out var set) ? set : [];

    public IEnumerable<EntityId> GetEntities(IEnumerable<ChunkCoord> chunks) =>
        chunks.SelectMany(c => GetEntities(c));

    public HashSet<ChunkCoord> GetNeighborhood(ChunkCoord center, int radius = 2)
    {
        var result = new HashSet<ChunkCoord>();
        for (short dx = (short)-radius; dx <= radius; dx++)
            for (short dy = (short)-radius; dy <= radius; dy++)
                result.Add(new ChunkCoord((short)(center.X + dx), (short)(center.Y + dy)));
        return result;
    }

    private void AddToChunk(EntityId entity, ChunkCoord chunk)
    {
        if (!_chunks.ContainsKey(chunk))
            _chunks[chunk] = [];
        _chunks[chunk].Add(entity);
    }

    private void RemoveFromChunk(EntityId entity, ChunkCoord chunk)
    {
        if (_chunks.TryGetValue(chunk, out var set))
        {
            set.Remove(entity);
            if (set.Count == 0) _chunks.Remove(chunk);
        }
    }

    // ── Static helpers ────────────────────────────────────────────────

    public static bool IsTileBlocked(World world, Guid mapId, int x, int y)
    {
        if (!world.MapDefs.TryGetValue(mapId, out var m))
            return true;
        var chunk = FromPosition(x, y);
        if (!m.Chunks.TryGetValue(chunk, out var mc))
            return true;
        if (mc.Tiles == null)
            return true;
        var localX = x - chunk.X * ChunkSize;
        var localY = y - chunk.Y * ChunkSize;
        if (localX < 0 || localX >= ChunkSize || localY < 0 || localY >= ChunkSize)
            return true;
        return mc.Tiles[localX, localY].IsBlocked;
    }

    public static EntityId? FindEntityAtTile(World world, Guid mapId, int x, int y)
    {
        var chunk = FromPosition(x, y);
        foreach (var id in world.SpatialGrid.GetEntities(chunk))
        {
            var pos = world.Get<Position>(id);
            if (pos != null && pos.X == x && pos.Y == y && pos.MapId == mapId)
                return id;
        }
        return null;
    }

    public static EntityId? FindGroundItemAtTile(World world, Guid mapId, int x, int y)
    {
        var chunk = FromPosition(x, y);
        foreach (var id in world.SpatialGrid.GetEntities(chunk))
        {
            var pos = world.Get<Position>(id);
            if (pos != null && pos.X == x && pos.Y == y && pos.MapId == mapId)
            {
                if (world.Get<GroundItem>(id) != null)
                    return id;
            }
        }
        return null;
    }

    public static EntityId? FindSolidEntityAtTile(World world, Guid mapId, int x, int y)
    {
        var chunk = FromPosition(x, y);
        foreach (var id in world.SpatialGrid.GetEntities(chunk))
        {
            var pos = world.Get<Position>(id);
            if (pos != null && pos.X == x && pos.Y == y && pos.MapId == mapId)
            {
                if (world.Has<PlayerTag>(id) || world.Has<NpcTag>(id))
                    return id;
            }
        }
        return null;
    }
}
