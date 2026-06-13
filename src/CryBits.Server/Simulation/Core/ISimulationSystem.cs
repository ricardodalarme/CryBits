using CryBits.Server.World;
using CryBits.Simulation.Core;

namespace CryBits.Server.Simulation.Core;

internal interface ISimulationSystem
{
    void Execute(GameWorld world, Tick tick);
}
