using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Simulation.Core;

namespace CryBits.Client.Systems.Map;

internal sealed class FogSystem(World world) : IClientSystem
{
    public void Update(float deltaTime)
    {
        foreach (var state in world.All)
        {
            var fog = state.Get<FogComponent>();
            if (fog == null) continue;

            world.Set(state.Id, fog with
            {
                OffsetX = fog.OffsetX + fog.SpeedX * deltaTime,
                OffsetY = fog.OffsetY + fog.SpeedY * deltaTime
            });
        }
    }
}
