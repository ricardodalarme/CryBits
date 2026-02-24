using System.Drawing;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using CryBits.Client.Graphics;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Systems;

/// <summary>
/// Renders screen-locked scrolling sprites (fog, overlays).
/// Queries entities that have both <see cref="SpriteComponent"/> and
/// <see cref="ScrollingSpriteComponent"/> and draws them at screen (0, 0)
/// with no camera transform — they follow the viewport by definition.
/// </summary>
internal sealed class ScrollingOverlayRenderSystem(World world) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<SpriteComponent, ScrollingSpriteComponent>();

    private readonly Rectangle _screenDest = new(0, 0, ScreenWidth, ScreenHeight);

    public override void Update(in int t)
    {
        World.Query(in _query, (ref SpriteComponent sprite, ref ScrollingSpriteComponent scroll) =>
        {
            var source = new Rectangle(
                (int)scroll.ExactX,
                (int)scroll.ExactY,
                ScreenWidth,
                ScreenHeight);
            Renders.Render(sprite.Texture, source, _screenDest, sprite.Tint);
        });
    }
}
