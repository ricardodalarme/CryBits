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

            var newCountdown = attack.AttackCountdown > 0f
                ? MathF.Max(0f, attack.AttackCountdown - dt)
                : attack.AttackCountdown;

            if (attack.AttackCountdown != newCountdown)
                world.Set(state.Id, new AttackComponent(newCountdown));

            var frameY = movement.Direction switch
            {
                Direction.Up => MovementUp,
                Direction.Down => MovementDown,
                Direction.Left => MovementLeft,
                Direction.Right => MovementRight,
                _ => 0
            };

            if (attack.IsAttacking && newCountdown > AttackSpeed / 2000f)
                world.Set(state.Id, new AnimatedSpriteComponent(
                    anim.FrameWidth, anim.FrameHeight, anim.FrameCount, anim.TimePerFrame,
                    anim.Timer, AnimationAttack, frameY, false));
            else if (movement.IsMoving)
                world.Set(state.Id, anim with { CurrentFrameY = frameY, Playing = true });
            else
                world.Set(state.Id, anim with { CurrentFrameY = frameY, Playing = false, CurrentFrameX = 0 });
        }
    }
}
