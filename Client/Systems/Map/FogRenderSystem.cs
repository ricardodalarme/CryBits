using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Graphics;
using System.Drawing;
using static CryBits.Globals;

namespace CryBits.Client.Systems.Map;

/// <summary>
/// Renders the map's fog overlay at a fixed screen position with no camera transform,
/// so the fog follows the viewport rather than the world.
///
/// Queries the singleton fog entity (created by <see cref="Spawners.FogSpawner"/>)
/// and draws its texture using the scroll offset accumulated by <see cref="FogSystem"/>
/// as the source rect origin, producing the seamless infinite-panning illusion.
/// </summary>
internal sealed class FogRenderSystem(World world, Renderer renderer) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<SpriteComponent, FogComponent>();

    public override void Update(in int t)
    {
        var screenDest = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

        World.Query(in _query, (ref SpriteComponent sprite, ref FogComponent fog) =>
        {
            var source = new Rectangle(
                (int)fog.OffsetX,
                (int)fog.OffsetY,
                ScreenWidth,
                ScreenHeight);
            renderer.Draw(sprite.Texture, source, screenDest, sprite.Tint);
        });
    }
}
