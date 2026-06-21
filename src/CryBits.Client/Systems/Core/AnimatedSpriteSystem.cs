using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Simulation.Core;

namespace CryBits.Client.Systems.Core;

internal sealed class AnimatedSpriteSystem(World world) : IClientSystem
{
    public void Update(float deltaTime)
    {
        foreach (var state in world.All)
        {
            var anim = state.Get<AnimatedSpriteComponent>();
            if (anim == null || !anim.Playing) continue;

            anim.Timer += deltaTime;
            if (anim.Timer < anim.TimePerFrame) continue;

            anim.Timer -= anim.TimePerFrame;
            anim.CurrentFrameX++;

            if (anim.CurrentFrameX >= anim.FrameCount)
                anim.CurrentFrameX = 0;
        }
    }
}
