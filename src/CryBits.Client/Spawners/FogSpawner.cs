using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using Microsoft.Xna.Framework;

namespace CryBits.Client.Spawners;

internal static class FogSpawner
{
    public static void Spawn(World world, FogConfig? fog)
    {
        world.DestroyWhere(world.Has<FogComponent>);

        if (fog == null || fog.Texture == 0) return;
        if (Textures.Fogs[fog.Texture] is not { } texture) return;

        var color = new Color(255, 255, 255, (int)fog.Alpha);
        _ = world.SpawnBuilder()
            .With(new SpriteComponent(texture, null, color))
            .With(new FogComponent(fog.SpeedX, fog.SpeedY, 0f, 0f))
            .Id;
    }
}
