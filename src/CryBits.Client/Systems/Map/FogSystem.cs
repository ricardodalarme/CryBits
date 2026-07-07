using CryBits.Client.Components;
using CryBits.Simulation.Core;

namespace CryBits.Client.Systems.Map;

internal sealed class FogSystem(World world) : IClientSystem
{
    public void Update(float deltaTime)
    {
        foreach (var entityId in world.All)
        {
            var fog = world.Get<FogComponent>(entityId);
            if (fog == null) continue;

            world.Set(entityId, fog with
            {
                OffsetX = fog.OffsetX + fog.SpeedX * deltaTime,
                OffsetY = fog.OffsetY + fog.SpeedY * deltaTime
            });
        }
    }
}
