using CryBits.Client.Components;
using CryBits.Simulation.Core;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Rendering.Effects;

internal sealed class FogRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    public void Render()
    {
        var screenDest = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

        foreach (var entityId in world.All)
        {
            var sprite = world.Get<SpriteComponent>(entityId);
            var fog = world.Get<FogComponent>(entityId);
            if (sprite == null || fog == null) continue;

            var source = new Rectangle(
                (int)fog.OffsetX,
                (int)fog.OffsetY,
                ScreenWidth,
                ScreenHeight);
            spriteBatch.Draw(sprite.Texture, source, screenDest, sprite.Tint);
        }
    }
}
