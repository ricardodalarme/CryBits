using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Definitions.Characters;
using CryBits.Simulation.Core;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Character;

internal sealed class CharacterAnimationSystem(World world) : IClientSystem
{
    private const float WalkFrameTime = 0.15f;
    private const float AttackFrameTime = 0.375f;

    public void Update(float dt)
    {
        foreach (var state in world.All)
        {
            var anim = state.Get<AnimationState>();
            var movement = state.Get<MovementComponent>();
            if (anim == null || movement == null) continue;

            var sheet = SpriteSheet.Default;

            var attack = state.Get<AttackComponent>();
            if (attack != null && attack.AttackCountdown > 0f)
            {
                var cd = MathF.Max(0f, attack.AttackCountdown - dt);
                if (cd != attack.AttackCountdown)
                    world.Set(state.Id, new AttackComponent(cd));
            }

            var frameY = sheet.RowForDirection(movement.Direction);
            var showAttack = attack != null && attack.AttackCountdown > AttackFrameTime;

            var frameTime = WalkFrameTime * (WalkSpeedPixelsPerSecond / movement.SpeedPixelsPerSecond);
            var (current, frameX, timer) = Determine(anim, dt, movement.IsMoving, showAttack, sheet.Columns, frameTime);

            world.Set(state.Id, new AnimationState(frameX, frameY, timer, current));
        }
    }

    private static (CharacterAnimation, int FrameX, float Timer) Determine(
        AnimationState prev, float dt, bool isMoving, bool showAttack, int columns, float frameTime)
    {
        if (showAttack)
            return (CharacterAnimation.Attack, columns - 1, 0f);

        if (!isMoving)
            return (CharacterAnimation.Idle, 1, 0f);

        // Pendulum walk cycle: 0 → 1 → 2 → 1 → 0 → 1 → 2 → 1 ...
        var timer = prev.Current switch
        {
            CharacterAnimation.Idle => 0f,
            CharacterAnimation.Attack => frameTime,
            _ => prev.Timer + dt
        };
        var frameX = prev.Current switch
        {
            CharacterAnimation.Idle => 0,
            CharacterAnimation.Attack => 0,
            _ => prev.FrameX
        };
        var lastExtreme = frameX != 1 ? frameX : 0;
        while (timer >= frameTime)
        {
            timer -= frameTime;
            if (frameX == 0) { frameX = 1; lastExtreme = 0; }
            else if (frameX == columns - 1) { frameX = 1; lastExtreme = columns - 1; }
            else frameX = lastExtreme == 0 ? columns - 1 : 0;
        }
        return (CharacterAnimation.Walk, frameX, timer);
    }
}
