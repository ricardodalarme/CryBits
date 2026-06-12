using System.Collections.Generic;
using CryBits.Server.World;

namespace CryBits.Server.Simulation.Core;

internal sealed class TickPipeline
{
    private readonly List<ISimulationSystem> _systems = [];

    public void Add<T>() where T : ISimulationSystem, new()
    {
        _systems.Add(new T());
    }

    public TickPipeline AddSystem(ISimulationSystem system)
    {
        _systems.Add(system);
        return this;
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var system in _systems)
            system.Execute(world, tick);
    }
}
