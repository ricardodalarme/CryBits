using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using static CryBits.Simulation.SimulationConstants;
using CommonMovement = CryBits.Definitions.Common.Movement;

namespace CryBits.Simulation.Systems.Movement;

public sealed class PathFollowSystem : ISimulationSystem
{
    private long _lastTick;

    public void Execute(World world, Tick tick)
    {
        if (tick.TickNumber - _lastTick < TicksPerSecond / 2) return;
        _lastTick = tick.TickNumber;

        foreach (var entity in world.Entities.All)
        {
            var pathFollow = entity.Get<PathFollow>();
            if (pathFollow == null) continue;

            if (pathFollow.IsComplete)
            {
                entity.Remove<PathFollow>();
                continue;
            }

            var dir = pathFollow.Steps[pathFollow.NextIndex];
            tick.Intents.Enqueue(new MoveIntent(entity.Id, dir, CommonMovement.Walking));
            entity.Set(pathFollow with { NextIndex = pathFollow.NextIndex + 1 });
        }
    }
}
