using System;
using System.Linq;
using CryBits.Server.ECS.Components;

namespace CryBits.Server.ECS;

/// <summary>
/// Singleton gateway to the server-side ECS world.
/// Provides the shared <see cref="World"/> instance and common entity query helpers.
/// </summary>
internal sealed class ServerContext
{
    // ─── Singleton ──────────────────────────────────────────────────────────

    public static readonly ServerContext Instance = new();

    private ServerContext() { }

    // ─── Core ───────────────────────────────────────────────────────────────

    public readonly World World = new();

    // ─── Player helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Find a playing player entity by name.
    /// Searches <see cref="World.Query{T}"/> over <see cref="PlayerDataComponent"/>.
    /// Returns null when no match is found.
    /// </summary>
    internal Server.Entities.Player? FindPlayer(string name)
    {
        foreach (var session in World.Query<SessionComponent>())
        {
            var pd = World.TryGet<PlayerDataComponent>(session.Id, out var data)
                ? data : null;
            if (pd != null && pd.Name.Equals(name, StringComparison.Ordinal))
                return session.Component.Session.Character;
        }
        return null;
    }

    // ─── NPC helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the entity ID of the alive NPC at the given map position, or -1.
    /// </summary>
    internal int FindNpc(Guid mapId, byte x, byte y)
    {
        foreach (var (id, data, pos) in World.Query<NpcDataComponent, PositionComponent>())
        {
            if (data.MapId != mapId) continue;
            if (!World.Get<NpcStateComponent>(id).Alive) continue;
            if (pos.X == x && pos.Y == y) return id;
        }
        return -1;
    }

    // ─── MapItem helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Returns the entity ID of the map item at the given position, or -1.
    /// Searches from the end of the list (most-recently-dropped first).
    /// </summary>
    internal int FindMapItem(Guid mapId, byte x, byte y)
    {
        int result = -1;
        foreach (var (id, item) in World.Query<MapItemComponent>())
        {
            if (item.MapId != mapId) continue;
            if (item.X == x && item.Y == y) result = id;
        }
        return result;
    }

    /// <summary>Remove all map-item entities that belong to <paramref name="mapId"/>.</summary>
    internal void ClearMapItems(Guid mapId)
    {
        var toDestroy = World.Query<MapItemComponent>()
            .Where(p => p.Component.MapId == mapId)
            .Select(p => p.Id)
            .ToList();
        foreach (var id in toDestroy)
            World.Destroy(id);
    }

    /// <summary>Count of map items on a given map.</summary>
    internal int MapItemCount(Guid mapId)
    {
        var count = 0;
        foreach (var (_, item) in World.Query<MapItemComponent>())
            if (item.MapId == mapId) count++;
        return count;
    }

    /// <summary>Returns all <see cref="MapItemComponent"/> instances on a given map.</summary>
    internal System.Collections.Generic.List<MapItemComponent> GetMapItems(Guid mapId)
    {
        var result = new System.Collections.Generic.List<MapItemComponent>();
        foreach (var (_, item) in World.Query<MapItemComponent>())
            if (item.MapId == mapId) result.Add(item);
        return result;
    }
}
