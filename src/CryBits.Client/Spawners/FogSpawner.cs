using Arch.Core;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Framework.Graphics;
using CryBits.Entities.Map;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

/// <summary>
/// Creates (or replaces) the singleton fog entity for the current map.
/// The fog texture must have <c>Repeated = true</c> (set in <see cref="Textures"/>).
/// Position is fixed at screen (0, 0); scrolling is driven by <see cref="FogComponent"/>.
/// </summary>
internal static class FogSpawner
{
    private static readonly QueryDescription _fogQuery =
        new QueryDescription().WithAll<FogComponent>();

    public static void Spawn(World world, MapFog fog)
    {
        world.Destroy(in _fogQuery);

        if (fog.Texture == 0) return;
        var color = new Color(255, 255, 255, fog.Alpha);
        world.Create(
            new SpriteComponent(Textures.Fogs[fog.Texture], null, color),
            new FogComponent(fog.SpeedX, fog.SpeedY)
        );
    }
}
