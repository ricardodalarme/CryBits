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
        var delta = dt;
        World.Query(in _query, (ref SpriteComponent sprite, ref DamageTintComponent damage) =>
        {
            // Count down the hurt tint duration.
            if (damage.IsHurt)
            {
                damage.HurtCountdown -= delta;
                if (damage.HurtCountdown <= 0f)
                {
                    damage.HurtCountdown = 0f;
                    damage.IsHurt = false;
                }
            }

            sprite.Tint = damage.IsHurt ? new Color(205, 125, 125) : Color.White;
        });
    }
}
