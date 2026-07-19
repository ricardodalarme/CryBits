using CryBits.Simulation.Core;

namespace CryBits.Client.Systems;

internal sealed class SystemScheduler
{
    private readonly List<IClientSystem> _systems = [];

    public SystemScheduler AddSimulation(IClientSystem s)
    {
        _systems.Add(s);
        return this;
    }

    public void Update(World world, float dt)
    {
        foreach (var s in _systems)
        {
            s.Update(world, dt);
        }
    }
}
