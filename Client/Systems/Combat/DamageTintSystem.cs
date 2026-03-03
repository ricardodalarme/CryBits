using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using SFML.Graphics;

namespace CryBits.Client.Systems.Combat;

internal sealed class DamageTintSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<SpriteComponent, DamageTintComponent>();

    public override void Update(in float dt)
    {
        var deltaTime = dt;

        World.Query(in _query, (ref SpriteComponent sprite, ref DamageTintComponent damage) =>
        {
            if (damage.IsHurt)
            {
                damage.TimeRemaining -= deltaTime;
                if (damage.TimeRemaining <= 0f)
                    damage.IsHurt = false;
            }

            sprite.Tint = damage.IsHurt ? new Color(205, 125, 125) : Color.White;
        });
    }
}
