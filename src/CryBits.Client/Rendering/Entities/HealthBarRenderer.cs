using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Characters;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;

namespace CryBits.Client.Rendering.Entities;

internal sealed class HealthBarRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    public void Render()
    {
        foreach (var state in world.All)
        {
            var transform = state.Get<TransformComponent>();
            var vitals = state.Get<Vitals>();
            var anim = state.Get<AnimationState>();
            var sprite = state.Get<SpriteComponent>();
            if (transform == null || vitals == null || anim == null || sprite == null) continue;

            var hp = vitals.Hp;
            var maxHp = vitals.MaxHp;

            if (hp <= 0 || hp >= maxHp) continue;

            var sheet = SpriteSheet.Default;
            var textureSize = sprite.Texture.ToSize();
            var frameW = sheet.FrameW(textureSize.Width);
            var frameH = sheet.FrameH(textureSize.Height);

            var barX = transform.X;
            var barY = transform.Y + frameH + 4;
            var fullWidth = frameW;
            var fillWidth = hp * fullWidth / maxHp;

            spriteBatch.Draw(Textures.Bars, barX, barY, 0, 4, fullWidth, 4);
            spriteBatch.Draw(Textures.Bars, barX, barY, 0, 0, fillWidth, 4);
        }
    }
}
