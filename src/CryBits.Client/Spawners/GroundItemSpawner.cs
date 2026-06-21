using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Spawners;

internal static class GroundItemSpawner
{
    public static void Spawn(World world, Item item, byte tileX, byte tileY)
    {
        var texture = Textures.Items[item.Texture];

        _ = world.SpawnBuilder()
            .With(new TransformComponent { X = tileX * Grid, Y = tileY * Grid })
            .With(new SpriteComponent { Texture = texture })
            .With(new GroundItem { ItemDefId = item.Id, Amount = 1, DespawnTick = -1 })
            .Id;
    }
}
