using System.Drawing;
using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Client.Spawners;

/// <summary>
/// Spawns blood-splatter entities into the ECS world.
/// The random sprite-sheet frame is baked into <see cref="SpriteComponent.SourceRect"/>
/// at spawn time, so no blood-specific component is needed.
/// </summary>
internal static class BloodSplatSpawner
{
    private const int FrameSize = 32;

    public static void Spawn(Arch.Core.World world, short tileX, short tileY)
    {
        var frame = MyRandom.Next(0, 3);
        var sourceRect = new Rectangle(frame * FrameSize, 0, FrameSize, FrameSize);

        world.Create(
            new TransformComponent(tileX * Grid, tileY * Grid),
            new SpriteComponent(Textures.Blood, sourceRect),
            new FadeComponent(intervalSeconds: 0.1f, amountPerTick: 1)
        );
    }
}
