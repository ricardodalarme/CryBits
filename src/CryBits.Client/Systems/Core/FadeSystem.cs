using CryBits.Client.Components;
using CryBits.Simulation.Core;
using Microsoft.Xna.Framework;

namespace CryBits.Client.Systems.Core;

internal sealed class FadeSystem : IClientSystem
{
    public void Update(World world, float deltaTime)
    {
        var commands = new CommandBuffer(world);

        foreach (var entityId in world.All)
        {
            var sprite = world.Get<SpriteComponent>(entityId);
            var fade = world.Get<FadeComponent>(entityId);
            if (sprite == null || fade == null) continue;

            var newTimer = fade.Timer - deltaTime;
            if (newTimer > 0)
            {
                world.Set(entityId, fade with { Timer = newTimer });
                continue;
            }

            var newAlpha = (byte)Math.Max(0, sprite.Tint.A - fade.AmountPerTick);
            world.Set(entityId,
                sprite with { Tint = new Color(sprite.Tint.R, sprite.Tint.G, sprite.Tint.B, newAlpha) });
            world.Set(entityId, fade with { Timer = fade.IntervalSeconds });

            if (newAlpha == 0)
                commands.Destroy(entityId);
        }

        commands.Flush();
    }
}
