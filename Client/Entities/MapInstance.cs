using CryBits.Client.ECS;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Entities.Map;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Utils;

namespace CryBits.Client.Entities;

/// <summary>
/// Runtime instance of a loaded map.
///
/// Holds the static map definition (<see cref="Data"/>) and the two
/// map-level simulations (<see cref="Weather"/>, <see cref="Fog"/>) that are
/// not entity-driven.
///
/// Previously, this class also owned <c>NpcInstance[]</c>, <c>MapItemInstance[]</c>
/// and a blood-splatter list.  Those collections have been replaced by ECS entities
/// managed by <see cref="GameContext"/> — query the <see cref="GameContext.World"/>
/// for <see cref="NpcDataComponent"/>, <see cref="MapItemComponent"/> and
/// <see cref="BloodSplatComponent"/> instead.
/// </summary>
internal sealed class MapInstance
{
    public readonly Map Data;
    public MapWeatherInstance Weather { get; }
    public MapFogInstance Fog { get; }

    public MapInstance(Map data)
    {
        Data = data;
        Weather = new MapWeatherInstance(data.Weather);
        Fog = new MapFogInstance(data.Fog);
    }

    /// <summary>
    /// Returns true when the tile in the given direction from (x, y) cannot be entered.
    /// Collision checks against players and NPCs are done via the ECS world.
    /// </summary>
    public bool TileBlocked(GameContext ctx, byte x, byte y, Direction direction)
    {
        byte nextX = x, nextY = y;
        NextTile(direction, ref nextX, ref nextY);

        // Leaving the map — only passable when a link exists.
        if (Map.OutLimit(nextX, nextY)) return Data.Link[(byte)direction] == null;

        // Tile attribute blocks.
        if (Data.Attribute[nextX, nextY].Type == (byte)TileAttribute.Block) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;

        // Occupancy checks via ECS.
        if (HasPlayer(ctx, nextX, nextY)) return true;
        if (HasNpc(ctx, nextX, nextY)) return true;

        return false;
    }

    // ─── Private occupancy checks ────────────────────────────────────────────

    private bool HasPlayer(GameContext ctx, byte x, byte y)
    {
        foreach (var (id, transform) in ctx.World.Query<TransformComponent>())
        {
            if (!ctx.World.Has<PlayerDataComponent>(id)) continue;
            if (!ctx.World.TryGet<MapContextComponent>(id, out var mc)) continue;
            if (mc.MapId != Data.Id) continue;
            if (transform.TileX == x && transform.TileY == y) return true;
        }
        return false;
    }

    private bool HasNpc(GameContext ctx, byte x, byte y)
    {
        foreach (var (_, npc, transform) in ctx.World.Query<NpcDataComponent, TransformComponent>())
        {
            if (npc.Data == null) continue;
            if (transform.TileX == x && transform.TileY == y) return true;
        }
        return false;
    }
}
