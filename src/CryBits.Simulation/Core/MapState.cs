using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Entities;
using CryBits.Simulation.State;
using System;
using System.Collections.Generic;

namespace CryBits.Simulation.Core;

public sealed class MapState
{
    public Guid Id { get; }
    public Map Data { get; }
    public List<EntityId> NpcIds { get; } = [];
    public List<GroundItem> GroundItems { get; } = [];

    public MapState(Guid id, Map data)
    {
        Id = id;
        Data = data;
    }

    public EntityId? HasNpc(byte x, byte y, EntityRegistry entities)
    {
        foreach (var npcId in NpcIds)
        {
            var entityState = entities.Get(npcId);
            if (entityState == null) continue;
            var npcState = entityState.Get<NpcState>();
            if (npcState == null || !npcState.Alive) continue;
            var pos = entityState.Get<Position>();
            if (pos == null) continue;
            if (pos.X == x && pos.Y == y)
                return npcId;
        }
        return null;
    }

    public EntityId? HasPlayer(byte x, byte y, EntityRegistry entities)
    {
        foreach (var state in entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var pos = state.Get<Position>();
            if (pos == null) continue;
            if (pos.X == x && pos.Y == y && pos.MapId == Id)
                return state.Id;
        }
        return null;
    }

    public bool HasPlayers(EntityRegistry entities)
    {
        foreach (var state in entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var pos = state.Get<Position>();
            if (pos == null) continue;
            if (pos.MapId == Id)
                return true;
        }
        return false;
    }

    public GroundItem? HasItem(byte x, byte y)
    {
        for (var i = GroundItems.Count - 1; i >= 0; i--)
            if (GroundItems[i].X == x && GroundItems[i].Y == y)
                return GroundItems[i];
        return null;
    }

    public void SpawnItems()
    {
        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (Data.Attribute[x, y].Type == (byte)TileAttribute.Item)
                    GroundItems.Add(new GroundItem(new Guid(Data.Attribute[x, y].Data1),
                        Data.Attribute[x, y].Data2, x, y));
    }

    public bool TileBlocked(byte x, byte y, Direction direction, EntityRegistry entities, bool countEntities = true)
    {
        byte nextX = x, nextY = y;
        direction.NextTile(ref nextX, ref nextY);

        if (Data.TileBlocked(nextX, nextY)) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)direction.Reverse()]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (countEntities && (HasPlayer(nextX, nextY, entities) != null || HasNpc(nextX, nextY, entities) != null)) return true;
        return false;
    }
}
