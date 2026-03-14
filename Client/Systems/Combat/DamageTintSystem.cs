using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using SFML.Graphics;
using System;

namespace CryBits.Client.Systems.Combat;

internal sealed class DamageTintSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<SpriteComponent, DamageTintComponent>();

    public override void Update(in float dt)
    {
        var now = Environment.TickCount;

        World.Query(in _query, (ref SpriteComponent sprite, ref DamageTintComponent damage) =>
        {
            // Auto-clear the hurt flag 325 ms after the hit was received.
            if (damage.IsHurt && damage.HurtTimestamp + 325 < now)
                damage.IsHurt = false;

            sprite.Tint = damage.IsHurt ? new Color(205, 125, 125) : Color.White;
        });
    }
}
