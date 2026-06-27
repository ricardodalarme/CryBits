using CryBits.Definitions.Catalog;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Spawners;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class GroundItemSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            if (ev is LootDroppedEvent loot)
                GroundItemSpawner.Spawn(world, catalog, loot.MapId, loot.X, loot.Y,
                    loot.ItemId, loot.Amount, loot.DespawnTick);
        }

        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers(world.Entities)) continue;

            for (var i = map.GroundItemIds.Count - 1; i >= 0; i--)
            {
                var entity = world.Entities.Get(map.GroundItemIds[i]);
                var groundItem = entity?.Get<GroundItem>();
                if (groundItem == null || groundItem.DespawnTick < 0) continue;

                if (tick.TickNumber >= groundItem.DespawnTick)
                {
                    tick.Events.Emit(new GroundItemRemovedEvent(tick.TickNumber, map.GroundItemIds[i], map.Id));
                    world.Entities.Destroy(map.GroundItemIds[i]);
                    map.GroundItemIds.RemoveAt(i);
                }
            }
        }
    }
}
