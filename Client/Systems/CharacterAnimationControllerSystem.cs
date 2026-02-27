using System;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Systems;

/// <summary>
/// Translates high-level RPG character states into raw animation frames.
/// Acts as the "Brain" controlling the "Muscles" (AnimatedSpriteComponent).
/// </summary>
internal sealed class CharacterAnimationControllerSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<CharacterStateComponent, AnimatedSpriteComponent>();

    public override void Update(in float dt)
    {
        var now = Environment.TickCount;

        World.Query(in _query, (ref CharacterStateComponent state, ref AnimatedSpriteComponent anim) =>
        {
            // 1. Set the Row based on Direction
            anim.CurrentFrameY = state.Direction switch
            {
                Direction.Up => MovementUp,
                Direction.Down => MovementDown,
                Direction.Left => MovementLeft,
                Direction.Right => MovementRight,
                _ => 0
            };

            // 2. Set the Column and Playback based on State
            if (state.IsAttacking && state.AttackTimer + AttackSpeed / 2 > now)
            {
                anim.Playing = false; // Stop walking animation
                anim.CurrentFrameX = AnimationAttack; // Force the attack frame
            }
            else if (state.IsMoving)
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
