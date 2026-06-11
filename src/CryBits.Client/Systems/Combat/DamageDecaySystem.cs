using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Combat;

namespace CryBits.Client.Systems.Combat;

/// <summary>
/// Translates high-level RPG character states into raw animation frames.
/// Acts as the "Brain" controlling the "Muscles" (AnimatedSpriteComponent).
/// </summary>
internal sealed class DamageDecaySystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription().WithAll<HurtComponent>();

    public override void Update(in float dt)
    {
        var delta = dt;

        // Tick down hurt cooldown
        World.Query(in _query, (Entity entity, ref HurtComponent damage) =>
        {
            damage.HurtCountdown -= delta;
            if (damage.HurtCountdown <= 0f) World.Remove<HurtComponent>(entity);
        });
    }
}
