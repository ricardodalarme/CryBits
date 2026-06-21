using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Graphics;
using CryBits.Simulation.Core;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Map;

internal sealed class FogRenderSystem(World world, Renderer renderer) : IClientRenderSystem
{
    public void Render()
    {
        var screenDest = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

        foreach (var state in world.All)
        {
            var sprite = state.Get<SpriteComponent>();
            var fog = state.Get<FogComponent>();
            if (sprite == null || fog == null) continue;

            var source = new Rectangle(
                (int)fog.OffsetX,
                (int)fog.OffsetY,
                ScreenWidth,
                ScreenHeight);
            renderer.Draw(sprite.Texture, source, screenDest, sprite.Tint);
        }
    }
}
