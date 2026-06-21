using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Definitions.Common;
using CryBits.Simulation.Core;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Character;

internal sealed class CharacterAnimationControllerSystem(World world) : IClientSystem
{
    public void Update(float dt)
    {
        foreach (var state in world.All)
        {
            var attack = state.Get<AttackComponent>();
            var anim = state.Get<AnimatedSpriteComponent>();
            var movement = state.Get<MovementComponent>();
            if (attack == null || anim == null || movement == null) continue;

            if (attack.AttackCountdown > 0f)
            {
                attack.AttackCountdown -= dt;
                if (attack.AttackCountdown <= 0f)
                    attack.AttackCountdown = 0f;
            }

            anim.CurrentFrameY = movement.Direction switch
            {
                Direction.Up => MovementUp,
                Direction.Down => MovementDown,
                Direction.Left => MovementLeft,
                Direction.Right => MovementRight,
                _ => 0
            };

            if (attack.IsAttacking && attack.AttackCountdown > AttackSpeed / 2000f)
            {
                anim.Playing = false;
                anim.CurrentFrameX = AnimationAttack;
            }
            else if (movement.IsMoving)
            {
                anim.Playing = true;
            }
            else
            {
                anim.Playing = false;
                anim.CurrentFrameX = 0;
            }
        }
    }
}
