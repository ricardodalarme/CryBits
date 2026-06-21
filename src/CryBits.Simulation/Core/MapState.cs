using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Core;

public sealed class MapState
{
    public Guid Id { get; }
    public Map Data { get; }
    public List<EntityId> NpcIds { get; } = [];
    public List<EntityId> GroundItemIds { get; } = [];

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
            if (npcState == null) continue;
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

    public EntityId? FindGroundItemEntity(EntityRegistry entities, byte x, byte y)
    {
        foreach (var entityId in GroundItemIds)
        {
            var entityState = entities.Get(entityId);
            if (entityState == null) continue;
            var pos = entityState.Get<Position>();
            if (pos == null) continue;
            if (pos.X == x && pos.Y == y)
                return entityId;
        }
        return null;
    }

    public void SpawnItems(EntityRegistry entities)
    {
        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (Data.Attribute[x, y].Type == (byte)TileAttribute.Item)
                {
                    var entityId = entities.Create();
                    var entity = entities.Get(entityId)!;
                    entity.Set(new Position { MapId = Id, X = x, Y = y });
                    entity.Set(new GroundItem
                    {
                        ItemDefId = new Guid(Data.Attribute[x, y].Data1),
                        Amount = Data.Attribute[x, y].Data2,
                        DespawnTick = -1
                    });
                    GroundItemIds.Add(entityId);
                }
    }

    public bool TileBlocked(byte x, byte y, Direction direction, EntityRegistry entities, bool countEntities = true)
    {
        var (nextX, nextY) = direction.NextTile(x, y);

        if (Data.TileBlocked(nextX, nextY)) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)direction.Reverse()]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (countEntities && (HasPlayer(nextX, nextY, entities) != null || HasNpc(nextX, nextY, entities) != null)) return true;
        return false;
    }
}
