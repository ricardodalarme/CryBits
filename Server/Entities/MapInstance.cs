using System;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Systems;
using CryBits.Server.World;
using static CryBits.Utils;

namespace CryBits.Server.Entities;

internal class MapInstance(Guid id, Map map) : Entity(id)
{
    // Map data reference (pure definition, no runtime state).
    public readonly Map Data = map;

    // ─── Entity queries ──────────────────────────────────────────────────────

    /// <summary>Returns the alive NPC entity ID at (x, y), or -1 when none.</summary>
    public int HasNpc(byte x, byte y) =>
        ServerContext.Instance.FindNpc(Data.Id, x, y);

    /// <summary>Returns the player standing at (x, y) on this map, or null.</summary>
    public Player? HasPlayer(byte x, byte y)
    {
        var world = ServerContext.Instance.World;
        foreach (var session in GameWorld.Current.Sessions)
        {
            if (!session.IsPlaying) continue;
            var player = session.Character!;
            var pos = world.Get<PositionComponent>(player.EntityId);
            if (pos.MapId == Data.Id && pos.X == x && pos.Y == y) return player;
        }

        return null;
    }

    /// <summary>Returns true when at least one player is on this map.</summary>
    public bool HasPlayers()
    {
        var world = ServerContext.Instance.World;
        foreach (var session in GameWorld.Current.Sessions)
        {
            if (!session.IsPlaying) continue;
            var pos = world.Get<PositionComponent>(session.Character!.EntityId);
            if (pos.MapId == Data.Id) return true;
        }

        return false;
    }

    /// <summary>Returns the map-item entity ID at (x, y), or -1 when none.</summary>
    public int HasItem(byte x, byte y) =>
        ServerContext.Instance.FindMapItem(Data.Id, x, y);

    // ─── Tile logic ──────────────────────────────────────────────────────────

    public bool TileBlocked(byte x, byte y, Direction direction, bool countEntities = true)
    {
        byte nextX = x, nextY = y;

        NextTile(direction, ref nextX, ref nextY);

        if (Data.TileBlocked(nextX, nextY)) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (countEntities && (HasPlayer(nextX, nextY) != null || HasNpc(nextX, nextY) >= 0)) return true;
        return false;
    }

    // ─── Item spawn ──────────────────────────────────────────────────────────

    /// <summary>
    /// Creates ECS map-item entities for all item-attribute tiles on this map.
    /// Called once during map creation and after the respawn timer fires.
    /// </summary>
    public void SpawnItems()
    {
        var world = ServerContext.Instance.World;
        for (byte x = 0; x < Map.Width; x++)
        for (byte y = 0; y < Map.Height; y++)
        {
            var attr = Data.Attribute[x, y];
            if (attr.Type != (byte)TileAttribute.Item) continue;

            var item = CryBits.Entities.Item.List.Get(new Guid(attr.Data1));
            if (item == null) continue;

            var entityId = world.Create();
            world.Add(entityId, new MapItemComponent
            {
                Item = item,
                Amount = (short)attr.Data2,
                X = x,
                Y = y,
                MapId = Data.Id
            });
        }
    }

    // ─── Factory ─────────────────────────────────────────────────────────────

    /// <summary>Creates a new MapInstance, registers it in GameWorld, and spawns NPCs and items.</summary>
    public static void Create(Map map, bool isOriginal)
    {
        var id = isOriginal ? map.Id : Guid.NewGuid();
        var tempMap = new MapInstance(id, map);
        GameWorld.Current.Maps.Add(id, tempMap);

        var world = ServerContext.Instance.World;

        // Create NPC entities.
        for (byte i = 0; i < map.Npc.Count; i++)
        {
            if (map.Npc[i].Npc == null) continue;

            var entityId = world.Create();
            world.Add(entityId, new NpcDataComponent { Data = map.Npc[i].Npc, Index = i, MapId = id });
            world.Add(entityId, new PositionComponent { MapId = id });
            world.Add(entityId, new DirectionComponent());
            world.Add(entityId, new VitalsComponent());
            world.Add(entityId, new NpcStateComponent());
            world.Add(entityId, new NpcTimerComponent());
            world.Add(entityId, new NpcTargetComponent());
            NpcAiSystem.Spawn(entityId);
        }

        // Spawn map items.
        tempMap.SpawnItems();
    }
}
