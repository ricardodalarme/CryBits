using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Spawners;

internal static class BloodSplatSpawner
{
    private const int FrameSize = 32;

    public static EntityId Spawn(World world, short tileX, short tileY)
    {
        var frame = Random.Shared.Next(0, 3);
        var sourceRect = new Rectangle(frame * FrameSize, 0, FrameSize, FrameSize);

        return world.SpawnBuilder()
            .With(new TransformComponent { X = tileX * Grid, Y = tileY * Grid })
            .With(new SpriteComponent { Texture = Textures.Blood, SourceRect = sourceRect })
            .With(new FadeComponent { IntervalSeconds = 0.1f, AmountPerTick = 1 })
            .Id;
    }
}
