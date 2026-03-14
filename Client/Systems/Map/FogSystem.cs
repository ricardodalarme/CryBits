using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Map;

namespace CryBits.Client.Systems.Map;

/// <summary>
/// Advances the scroll offset of the map's fog overlay each frame.
/// Pairs with <see cref="FogRenderSystem"/>, which reads the accumulated
/// offset to compute the source rect drawn to the screen.
/// </summary>
internal sealed class FogSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<FogComponent>();

    public override void Update(in float deltaTime)
    {
        var dt = deltaTime;
        World.Query(in _query, (ref FogComponent fog) =>
        {
            fog.OffsetX += fog.SpeedX * dt;
            fog.OffsetY += fog.SpeedY * dt;
        });
    }
}
