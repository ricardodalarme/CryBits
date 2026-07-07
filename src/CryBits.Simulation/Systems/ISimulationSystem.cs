using CryBits.Simulation.Core;

namespace CryBits.Simulation.Systems;

public interface ISimulationSystem
{
    void Execute(World world, Tick tick);
}
