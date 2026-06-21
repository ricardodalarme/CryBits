using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;

namespace CryBits.Client.Systems.Character;

internal sealed class VitalBarRenderSystem(World world, Renderer renderer) : IClientRenderSystem
{
    public void Render()
    {
        foreach (var state in world.All)
        {
            var transform = state.Get<TransformComponent>();
            var vitals = state.Get<Vitals>();
            var anim = state.Get<AnimatedSpriteComponent>();
            if (transform == null || vitals == null || anim == null) continue;

            var hp = vitals.Hp;
            var maxHp = vitals.MaxHp;

            if (hp <= 0 || hp >= maxHp) continue;

            var barX = transform.X;
            var barY = transform.Y + anim.FrameHeight + 4;
            var fullWidth = anim.FrameWidth;
            var fillWidth = hp * fullWidth / maxHp;

            renderer.Draw(Textures.Bars, barX, barY, 0, 4, fullWidth, 4);
            renderer.Draw(Textures.Bars, barX, barY, 0, 0, fillWidth, 4);
        }
    }
}
