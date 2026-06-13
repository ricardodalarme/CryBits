using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class GroundItemSystem : ISimulationSystem
{
    private long _lastCleanTick;

    public void Execute(World world, Tick tick)
    {
        if (tick.TickNumber - _lastCleanTick < TicksPerSecond * 300) return;
        _lastCleanTick = tick.TickNumber;

        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers(world.Entities)) continue;

            map.GroundItems.Clear();
            map.SpawnItems();
            world.CurrentTick?.Events.Emit(new MapGroundItemsChangedEvent { MapId = map.Id });
        }
    }
}
