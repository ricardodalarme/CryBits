using CryBits.Definitions.Catalog;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.State;
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

        var toDestroy = new List<EntityId>();
        foreach (var state in world.Entities.All)
        {
            var groundItem = state.Get<GroundItem>();
            if (groundItem == null || groundItem.DespawnTick < 0) continue;

            if (tick.TickNumber >= groundItem.DespawnTick)
                toDestroy.Add(state.Id);
        }

        foreach (var id in toDestroy)
            world.Destroy(id);
    }
}
