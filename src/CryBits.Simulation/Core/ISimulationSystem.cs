namespace CryBits.Simulation.Core;

public interface ISimulationSystem
{
    void Execute(World world, Tick tick);
}
