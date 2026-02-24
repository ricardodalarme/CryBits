using System.Drawing;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components;

namespace CryBits.Client.Systems;

internal sealed class AnimatedSpriteSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<AnimatedSpriteComponent, SpriteComponent>();

    public override void Update(in float deltaTime)
    {
        var dt = deltaTime;
        World.Query(in _query, (ref AnimatedSpriteComponent anim, ref SpriteComponent sprite) =>
        {
            if (anim.Playing)
            {
                anim.Timer += dt;
                if (anim.Timer >= anim.TimePerFrame)
                {
                    anim.Timer -= anim.TimePerFrame;
                    anim.CurrentFrameX++;

                    // Loop back to the start of the row
                    if (anim.CurrentFrameX >= anim.FrameCount)
                        anim.CurrentFrameX = 0;
                }
            }

            sprite.SourceRect = new Rectangle(
                anim.CurrentFrameX * anim.FrameWidth,
                anim.CurrentFrameY * anim.FrameHeight,
                anim.FrameWidth,
                anim.FrameHeight
            );
        });
    }
}
