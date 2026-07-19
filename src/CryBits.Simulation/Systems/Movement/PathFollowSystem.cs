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

        var commands = new CommandBuffer(world);

        foreach (var entity in world.Entities.All)
        {
            var pathFollow = world.Get<PathFollow>(entity);
            if (pathFollow == null) continue;

            if (pathFollow.IsComplete)
            {
                commands.Remove<PathFollow>(entity);
                continue;
            }

            var dir = pathFollow.Steps[pathFollow.NextIndex];
            tick.Intents.Enqueue(new MoveIntent(entity, dir, CommonMovement.Walking));
            commands.Set(entity, pathFollow with { NextIndex = pathFollow.NextIndex + 1 });
        }

        commands.Flush();
    }
}
