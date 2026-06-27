using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Spawners;

internal static class GroundItemSpawner
{
    public static EntityId Spawn(World world, long networkId, Item item, int tileX, int tileY)
    {
        var texture = Textures.Items[item.Texture];

        return world.SpawnBuilder()
            .With(new TransformComponent(tileX * Grid, tileY * Grid))
            .With(new SpriteComponent(texture, null, SFML.Graphics.Color.White))
            .With(new GroundItem(ItemDefId: item.Id, Amount: 1))
            .With(new NetworkId(networkId))
            .Id;
    }
}
