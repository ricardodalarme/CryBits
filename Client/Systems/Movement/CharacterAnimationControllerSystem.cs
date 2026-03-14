using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Movement;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Systems.Movement;

/// <summary>
/// Translates high-level RPG character states into raw animation frames.
/// Acts as the "Brain" controlling the "Muscles" (AnimatedSpriteComponent).
/// </summary>
internal sealed class CharacterAnimationControllerSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<AttackComponent, AnimatedSpriteComponent, MovementComponent>();

    private readonly QueryDescription _damageQuery = new QueryDescription().WithAll<DamageComponent>();

    public override void Update(in float dt)
    {
        var delta = dt;

        // Tick down hurt cooldown
        World.Query(in _damageQuery, (Entity entity, ref DamageComponent damage) =>
        {
            damage.HurtCountdown -= delta;
            if (damage.HurtCountdown <= 0f) World.Remove<DamageComponent>(entity);
        });

        World.Query(in _query, (ref AttackComponent state, ref AnimatedSpriteComponent anim, ref MovementComponent movement) =>
        {
            // Tick down attack cooldown
            if (state.AttackCountdown > 0f)
            {
                state.AttackCountdown -= delta;
                if (state.AttackCountdown <= 0f)
                {
                    state.AttackCountdown = 0f;
                }
            }

            // Set the Row based on Direction
            anim.CurrentFrameY = movement.Direction switch
            {
                Direction.Up => MovementUp,
                Direction.Down => MovementDown,
                Direction.Left => MovementLeft,
                Direction.Right => MovementRight,
                _ => 0
            };

            // Set the Column and Playback based on State
            if (state.IsAttacking && state.AttackCountdown > AttackSpeed / 2000f)
            {
                anim.Playing = false; // Stop walking animation
                anim.CurrentFrameX = AnimationAttack; // Force the attack frame
            }
            else if (movement.IsMoving)
            {
                anim.Playing = true; // Let the AnimatedSpriteSystem tick the walking frames
            }
            else
            {
                anim.Playing = false; // Stop walking animation
                anim.CurrentFrameX = 0; // Force the standing frame
            }
        });
    }
}
