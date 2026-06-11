using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;

namespace CryBits.Client.Systems.Core;

/// <summary>
/// Advances the animation frame timer for every entity that has an
/// <see cref="AnimatedSpriteComponent"/>.
/// </summary>
internal sealed class AnimatedSpriteSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<AnimatedSpriteComponent>();

    public override void Update(in float deltaTime)
    {
        var dt = deltaTime;
        World.Query(in _query, (ref AnimatedSpriteComponent anim) =>
        {
            if (!anim.Playing) return;

            anim.Timer += dt;
            if (anim.Timer < anim.TimePerFrame) return;

            anim.Timer -= anim.TimePerFrame;
            anim.CurrentFrameX++;

            // Loop back to the start of the row.
            if (anim.CurrentFrameX >= anim.FrameCount)
                anim.CurrentFrameX = 0;
        });
    }
}
