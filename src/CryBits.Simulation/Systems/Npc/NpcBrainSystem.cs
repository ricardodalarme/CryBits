using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Npc;

public sealed class NpcBrainSystem : ISimulationSystem
{
    private long _lastTick;

    public void Execute(World world, Tick tick)
    {
        if (tick.TickNumber - _lastTick < TicksPerSecond / 2) return;
        _lastTick = tick.TickNumber;

        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers(world.Entities)) continue;

            foreach (var npcId in map.NpcIds)
            {
                var e = world.Entities.Get(npcId);
                if (e == null) continue;
                var npcState = e.Get<NpcState>();
                if (npcState == null || !npcState.Alive) continue;
                TickAlive(world, npcId, tick);
            }
        }
    }

    private void TickAlive(World world, EntityId npcId, Tick tick)
    {
        NpcTargeting.UpdateTarget(world, npcId, tick);
        NpcMovement.TickMovement(world, npcId, tick);
    }
}
