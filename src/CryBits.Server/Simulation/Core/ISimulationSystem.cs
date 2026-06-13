using CryBits.Server.Core;
using CryBits.Simulation.Core;

namespace CryBits.Server.Simulation.Core;

internal interface ISimulationSystem
{
    void Execute(GameWorld world, Tick tick);
}
