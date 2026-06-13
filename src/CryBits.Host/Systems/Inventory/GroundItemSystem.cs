using CryBits.Host.Network.Senders;
using CryBits.Simulation.Core;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Host.Systems.Inventory;

internal sealed class GroundItemSystem(MapSender mapSender) : ISimulationSystem
{
    public static GroundItemSystem Instance { get; } = new(MapSender.Instance);

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
            mapSender.MapItems(map);
        }
    }
}
