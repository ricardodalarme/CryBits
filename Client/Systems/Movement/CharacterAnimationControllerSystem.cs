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
        .WithAll<AttackComponent, AnimatedSpriteComponent, DamageComponent, MovementComponent>();

    public override void Update(in float dt)
    {
        var delta = dt;
        World.Query(in _query, (ref AttackComponent state, ref AnimatedSpriteComponent anim, ref DamageComponent damage, ref MovementComponent movement) =>
        {
            // 1. Tick down hurt cooldown
            if (damage.IsHurt)
            {
                damage.HurtCountdown -= delta;
                if (damage.HurtCountdown <= 0f)
                {
                    damage.HurtCountdown = 0f;
                }
            }

            // 2. Tick down attack cooldown
            if (state.AttackCountdown > 0f)
            {
                state.AttackCountdown -= delta;
                if (state.AttackCountdown <= 0f)
                {
                    state.AttackCountdown = 0f;
                }
            }

            // 3. Set the Row based on Direction
            anim.CurrentFrameY = movement.Direction switch
            {
                Direction.Up => MovementUp,
                Direction.Down => MovementDown,
                Direction.Left => MovementLeft,
                Direction.Right => MovementRight,
                _ => 0
            };

            // 4. Set the Column and Playback based on State
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
