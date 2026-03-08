using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Graphics;
using SFML.Graphics;
using SFML.System;
using static CryBits.Globals;

namespace CryBits.Client.Systems.Core;

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

    private readonly IntRect _screenDest = new(new Vector2i(0, 0), new Vector2i(ScreenWidth, ScreenHeight));

    public override void Update(in int t)
    {
        World.Query(in _query, (ref SpriteComponent sprite, ref ScrollingSpriteComponent scroll) =>
        {
            var source = new IntRect(
                new Vector2i((int)scroll.ExactX, (int)scroll.ExactY),
                new Vector2i(ScreenWidth, ScreenHeight));
            Renderer.Instance.Draw(sprite.Texture, source, _screenDest, sprite.Tint);
        });
    }
}
