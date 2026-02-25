using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using SFML.Graphics;

namespace CryBits.Client.Systems;

internal sealed class DamageTintSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<SpriteComponent, DamageTintComponent>();

    public override void Update(in float dt)
    {
        World.Query(in _query, (ref SpriteComponent sprite, ref DamageTintComponent damage) =>
        {
            sprite.Tint = damage.IsHurt ? new Color(205, 125, 125) : Color.White;
        });
    }
}
