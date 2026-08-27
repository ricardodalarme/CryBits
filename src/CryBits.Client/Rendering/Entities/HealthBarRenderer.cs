using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            var texture = sprite.Texture;
            var frameW = sheet.FrameW(texture.Width);
            var frameH = sheet.FrameH(texture.Height);

            var barX = transform.X;
            var barY = transform.Y + frameH + 4;
            var fillWidth = hp * frameW / maxHp;

            spriteBatch.Draw(Textures.Bars,
                new Rectangle(barX, barY, frameW, 4),
                new Rectangle(0, 4, frameW, 4),
                Color.White);
            spriteBatch.Draw(Textures.Bars,
                new Rectangle(barX, barY, fillWidth, 4),
                new Rectangle(0, 0, fillWidth, 4),
                Color.White);
        }
    }
}
