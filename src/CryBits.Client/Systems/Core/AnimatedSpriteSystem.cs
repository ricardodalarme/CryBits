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

            var newTimer = anim.Timer + deltaTime;
            if (newTimer < anim.TimePerFrame)
            {
                world.Set(state.Id, anim with { Timer = newTimer });
                continue;
            }

            var frames = (int)(newTimer / anim.TimePerFrame);
            var remainder = newTimer - frames * anim.TimePerFrame;
            var newFrameX = (anim.CurrentFrameX + frames) % anim.FrameCount;

            world.Set(state.Id, anim with { Timer = remainder, CurrentFrameX = newFrameX });
        }
    }
}
