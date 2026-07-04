using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Spawners;

public static class GroundItemSpawner
{
    public static EntityId? Spawn(World world,
        Guid mapId, int x, int y, Guid itemDefId, short amount, long despawnTick)
    {
        if (!world.MapDefs.TryGetValue(mapId, out _))
            return null;

        var entityId = world.Entities.Create();

        world.Set(entityId, new Position(MapId: mapId, X: x, Y: y, Direction: Direction.Down));
        world.Set(entityId, new GroundItem(itemDefId, amount, despawnTick));
        world.Set(entityId, new GroundItemTag());

        return entityId;
    }
}
