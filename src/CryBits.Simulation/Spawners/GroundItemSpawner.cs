using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
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

        world.Set(entityId, new Position(MapId: mapId, X: x, Y: y, Direction: Direction.Down));
        world.Set(entityId, new GroundItem(itemDefId, amount, despawnTick));

        map.GroundItemIds.Add(entityId);
        return entityId;
    }
}
