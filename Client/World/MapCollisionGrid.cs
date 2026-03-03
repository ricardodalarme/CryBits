using System;
using Arch.Core;
using CryBits.Entities.Map;

namespace CryBits.Client.Worlds;

/// <summary>
/// Per-map O(1) spatial occupancy grid.
///
/// Each cell stores the <see cref="Entity"/> currently standing on that tile
/// (or <see cref="Entity.Null"/> when empty). The grid is rebuilt every frame
/// by <see cref="CryBits.Client.Systems.Map.MapCollisionSystem"/> so
/// <see cref="TileBlocked"/> lookups are always a single array read with no ECS
/// queries involved.
///
/// Layout: row-major — index = y * <see cref="Map.Width"/> + x.
/// </summary>
internal sealed class MapCollisionGrid
{
    private readonly Entity[] _cells = new Entity[Map.Width * Map.Height];

    /// <summary>
    /// Resets every cell to <see cref="Entity.Null"/>.
    /// Called once per frame by <see cref="CryBits.Client.Systems.Map.MapCollisionSystem"/>
    /// before the grid is repopulated.
    /// </summary>
    public void Clear() => Array.Fill(_cells, Entity.Null);

    /// <summary>Records <paramref name="entity"/> as occupying tile (<paramref name="x"/>, <paramref name="y"/>).</summary>
    public void Set(byte x, byte y, Entity entity) =>
        _cells[y * Map.Width + x] = entity;

    /// <summary>Returns the entity on tile (<paramref name="x"/>, <paramref name="y"/>), or <see cref="Entity.Null"/>.</summary>
    public Entity Get(byte x, byte y) =>
        _cells[y * Map.Width + x];

    /// <summary>Returns <c>true</c> when any entity occupies tile (<paramref name="x"/>, <paramref name="y"/>).</summary>
    public bool IsOccupied(byte x, byte y) =>
        _cells[y * Map.Width + x] != Entity.Null;
}
