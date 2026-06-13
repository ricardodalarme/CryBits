using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class GroundItemSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
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
                    world.CurrentTick?.Events.Emit(new GroundItemRemovedEvent { EntityId = map.GroundItemIds[i].Value, MapId = map.Id });
                    world.Entities.Destroy(map.GroundItemIds[i]);
                    map.GroundItemIds.RemoveAt(i);
                }
            }
        }
    }
}
