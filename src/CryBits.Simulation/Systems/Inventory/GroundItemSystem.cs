using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Spawners;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class GroundItemSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        var commands = new CommandBuffer(world);

        foreach (var ev in tick.Events.Events)
            if (ev is LootDroppedEvent loot)
                GroundItemSpawner.Spawn(world,
                    loot.MapId, loot.X, loot.Y,
                    loot.ItemId, loot.Amount, loot.DespawnTick);

        foreach (var entityId in world.Entities.All)
        {
            var groundItem = world.Get<GroundItem>(entityId);
            if (groundItem == null || groundItem.DespawnTick < 0) continue;

            if (tick.TickNumber >= groundItem.DespawnTick)
                commands.Destroy(entityId);
        }

        commands.Flush();
    }
}
