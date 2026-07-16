using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;

namespace CryBits.Client.Rendering.Entities;

internal sealed class HealthBarRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    public void Render()
    {
        foreach (var entityId in world.All)
        {
            var transform = world.Get<TransformComponent>(entityId);
            var vitals = world.Get<Vitals>(entityId);
            var anim = world.Get<AnimationState>(entityId);
            var sprite = world.Get<SpriteComponent>(entityId);
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
            var fillWidth = hp * frameW / maxHp;

            spriteBatch.Draw(Textures.Bars, barX, barY, 0, 4, frameW, 4);
            spriteBatch.Draw(Textures.Bars, barX, barY, 0, 0, fillWidth, 4);
        }
    }
}
