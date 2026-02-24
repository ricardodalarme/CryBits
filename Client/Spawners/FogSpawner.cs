using Arch.Core;
using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Entities.Map;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

/// <summary>
/// Creates (or replaces) the singleton fog entity for the current map.
/// The fog texture must have <c>Repeated = true</c> (set in <see cref="Textures"/>).
/// Position is fixed at screen (0, 0); scrolling is driven by <see cref="ScrollingSpriteComponent"/>.
/// </summary>
internal static class FogSpawner
{
    private static readonly QueryDescription _scrollingQuery =
        new QueryDescription().WithAll<ScrollingSpriteComponent>();

    public static void Spawn(World world, MapFog fog)
    {
        world.Destroy(in _scrollingQuery);

        if (fog.Texture == 0) return;
        var color = new Color(255, 255, 255, fog.Alpha);
        world.Create(
            new SpriteComponent(Textures.Fogs[fog.Texture], null, color),
            new ScrollingSpriteComponent(fog.SpeedX, fog.SpeedY)
        );
    }
}
