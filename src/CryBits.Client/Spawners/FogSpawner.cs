using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

internal static class FogSpawner
{
    public static void Spawn(World world, MapFog fog)
    {
        world.DestroyWhere(s => s.Has<FogComponent>());

        if (fog.Texture == 0) return;
        var color = new Color(255, 255, 255, fog.Alpha);
        _ = world.SpawnBuilder()
            .With(new SpriteComponent { Texture = Textures.Fogs[fog.Texture], Tint = color })
            .With(new FogComponent { SpeedX = fog.SpeedX, SpeedY = fog.SpeedY })
            .Id;
    }
}
