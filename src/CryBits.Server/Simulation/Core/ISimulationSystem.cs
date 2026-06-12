using CryBits.Server.World;

namespace CryBits.Server.Simulation.Core;

internal interface ISimulationSystem
{
    void Execute(GameWorld world, Tick tick);
}
