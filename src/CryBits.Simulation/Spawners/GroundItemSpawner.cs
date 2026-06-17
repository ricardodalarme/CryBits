using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Spawners;

public static class GroundItemSpawner
{
    public static EntityId? Spawn(World world, DefinitionCatalog catalog,
        Guid mapId, byte x, byte y, Guid itemDefId, short amount, long despawnTick)
    {
        var map = world.Maps.Get(mapId);
        if (map == null) return null;

        var entityId = world.Entities.Create();
        var entity = world.Entities.Get(entityId)!;

        entity.Set(new Position { MapId = mapId, X = x, Y = y });
        entity.Set(new GroundItem { ItemDefId = itemDefId, Amount = amount, DespawnTick = despawnTick });
        entity.Set(new GroundItemTag());

        map.GroundItemIds.Add(entityId);
        world.Dirty.Mark<GroundItem>(entityId);
        return entityId;
    }
}
