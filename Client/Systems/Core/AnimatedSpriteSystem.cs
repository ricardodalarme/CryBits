using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using SFML.Graphics;
using SFML.System;

namespace CryBits.Client.Systems.Core;

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

            sprite.SourceRect = new IntRect(
                new Vector2i(anim.CurrentFrameX * anim.FrameWidth, anim.CurrentFrameY * anim.FrameHeight),
                new Vector2i(anim.FrameWidth, anim.FrameHeight)
            );
        });
    }
}
