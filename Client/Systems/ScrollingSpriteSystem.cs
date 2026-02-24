using Arch.Core;
using Arch.System;
using CryBits.Client.Components;

namespace CryBits.Client.Systems;

/// <summary>
/// System that updates the position of scrolling sprites (e.g., fog) based on their speed and the elapsed time.
/// </summary>
internal sealed class ScrollingSpriteSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<ScrollingSpriteComponent>();

    public override void Update(in float deltaTime)
    {
        var dt = deltaTime;
        World.Query(in _query, (ref ScrollingSpriteComponent scroll) =>
        {
            scroll.ExactX += scroll.SpeedX * dt;
            scroll.ExactY += scroll.SpeedY * dt;
        });
    }
}
