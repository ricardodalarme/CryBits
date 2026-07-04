using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

internal static class FogSpawner
{
    public static void Spawn(World world, FogConfig? fog)
    {
        world.DestroyWhere(s => s.Has<FogComponent>());

        if (fog == null || fog.Texture == 0) return;
        var color = new Color(255, 255, 255, fog.Alpha);
        _ = world.SpawnBuilder()
            .With(new SpriteComponent(Textures.Fogs[fog.Texture], null, color))
            .With(new FogComponent(fog.SpeedX, fog.SpeedY, 0f, 0f))
            .Id;
    }
}
