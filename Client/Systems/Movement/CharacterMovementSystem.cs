using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Movement;
using CryBits.Enums;
using static CryBits.Globals;
using MovementState = CryBits.Enums.Movement;

namespace CryBits.Client.Systems.Movement;

/// <summary>
/// Advances the sub-tile pixel interpolation for every character entity each game tick.
/// This is the ECS equivalent of the old <c>Character.ProcessMovement()</c> method.
///
/// It owns three responsibilities:
///   1. Step <c>OffsetX/Y</c> toward zero at the configured speed (walking or running).
///   2. Clamp the offset when it crosses zero to prevent overshooting.
///   3. Detect tile arrival (offset reaches zero) and set <c>MovementState = Stopped</c>.
///
/// After the offset is finalised the system writes:
///   • <c>TransformComponent</c> — world-pixel position used by all render systems.
///   • <c>CharacterStateComponent.IsMoving</c> — consumed by <see cref="CharacterAnimationControllerSystem"/>.
///   • <c>CharacterStateComponent.Direction</c> — kept in sync with the movement component.
///
/// <b>Tick rate</b>: must run at the same 30 ms cadence as the legacy <c>Logic()</c> calls
/// so that the fixed 2 px/tick (walk) and 3 px/tick (run) speeds feel identical to
/// the original movement.
/// </summary>
internal sealed class CharacterMovementSystem(World world) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<CharacterMovementComponent, TransformComponent, CharacterStateComponent>();

    public override void Update(in int t)
    {
        World.Query(in _query,
            (ref CharacterMovementComponent movement,
             ref TransformComponent transform,
             ref CharacterStateComponent state) =>
            {
                Step(ref movement);

                transform.X = movement.TileX * Grid + movement.OffsetX;
                transform.Y = movement.TileY * Grid + movement.OffsetY;

                state.Direction = movement.Direction;
                state.IsMoving = movement.OffsetX != 0 || movement.OffsetY != 0;
            });
    }

    /// <summary>
    /// Advances the offset by one tick toward zero and detects tile arrival.
    /// Mirrors the exact clamping and stop logic from the legacy <c>ProcessMovement</c>.
    /// </summary>
    private static void Step(ref CharacterMovementComponent m)
    {
        if (m.MovementState == MovementState.Stopped)
        {
            m.OffsetX = 0;
            m.OffsetY = 0;
            return;
        }

        byte speed = m.MovementState == MovementState.Moving ? (byte)3 : (byte)2;

        short prevX = m.OffsetX, prevY = m.OffsetY;

        switch (m.Direction)
        {
            case Direction.Up: m.OffsetY -= speed; break;
            case Direction.Down: m.OffsetY += speed; break;
            case Direction.Right: m.OffsetX += speed; break;
            case Direction.Left: m.OffsetX -= speed; break;
        }

        // Clamp: prevent overshooting zero when the speed step crosses the boundary.
        if (prevX > 0 && m.OffsetX < 0) m.OffsetX = 0;
        if (prevX < 0 && m.OffsetX > 0) m.OffsetX = 0;
        if (prevY > 0 && m.OffsetY < 0) m.OffsetY = 0;
        if (prevY < 0 && m.OffsetY > 0) m.OffsetY = 0;

        // Arrival check — only stop once offset is fully consumed in the travel direction.
        bool arrived = m.Direction is Direction.Right or Direction.Down
            ? m.OffsetX >= 0 && m.OffsetY >= 0
            : m.OffsetX <= 0 && m.OffsetY <= 0;

        if (arrived)
            m.MovementState = MovementState.Stopped;
    }
}
