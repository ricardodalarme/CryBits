using Arch.Core;
using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Entities;
using static CryBits.Globals;

namespace CryBits.Client.Spawners;

/// <summary>
/// Spawns ground-item entities into the ECS world.
/// Each entity receives a <see cref="GroundItemComponent"/> (for game-logic interactions),
/// a <see cref="TransformComponent"/> (tile position) and a
/// <see cref="SpriteComponent"/> (full-texture rendering by <see cref="CryBits.Client.Systems.SpriteRenderSystem"/>).
/// </summary>
internal static class GroundItemSpawner
{
    public static void Spawn(World world, Item item, byte tileX, byte tileY)
    {
        var texture = Textures.Items[item.Texture];

        world.Create(
            new TransformComponent(tileX * Grid, tileY * Grid),
            new SpriteComponent(texture),
            new GroundItemComponent(item)
        );
    }
}
