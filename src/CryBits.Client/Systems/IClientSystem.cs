using CryBits.Simulation.Core;

namespace CryBits.Client.Systems;

public interface IClientSystem
{
    void Update(World world, float deltaTime);
}
