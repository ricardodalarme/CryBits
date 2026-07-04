using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Spawners;

internal static class BloodSplatSpawner
{
    private const int FrameSize = 32;

    public static EntityId Spawn(World world, int tileX, int tileY)
    {
        var frame = Random.Shared.Next(0, 3);
        var sourceRect = new Rectangle(frame * FrameSize, 0, FrameSize, FrameSize);

        return world.SpawnBuilder()
            .With(new TransformComponent(tileX * Grid, tileY * Grid))
            .With(new SpriteComponent(Textures.Blood, sourceRect, SFML.Graphics.Color.White))
            .With(new FadeComponent())
            .Id;
    }
}
