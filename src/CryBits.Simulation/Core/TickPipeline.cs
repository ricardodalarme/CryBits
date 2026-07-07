using CryBits.Simulation.Systems;

namespace CryBits.Simulation.Core;

public sealed class TickPipeline
{
    private readonly List<ISimulationSystem> _systems = [];

    public TickPipeline AddSystem(ISimulationSystem system)
    {
        _systems.Add(system);
        return this;
    }

    public void Execute(World world, Tick tick)
    {
        foreach (var system in _systems)
            system.Execute(world, tick);
    }
}
