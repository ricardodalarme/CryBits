using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Movement;
using CryBits.Enums;
using System;
using static CryBits.Globals;
using MovementState = CryBits.Enums.Movement;

namespace CryBits.Client.Systems.Movement;

/// <summary>
/// Advances the sub-tile pixel interpolation for every character entity each rendered frame.
/// Runs in the delta-time group so movement is perfectly smooth at any frame rate —
/// 30 Hz, 60 Hz, or 144 Hz all produce identical world-space speeds.
///
/// The interpolation speed comes from <see cref="MovementComponent.SpeedPixelsPerSecond"/>,
/// which is populated by the server via movement packets. This means speed buffs / debuffs
/// require no client code changes — the server simply sends a different speed value.
///
/// Responsibilities each frame:
///   1. Advance <c>OffsetX/Y</c> by <c>speed × dt</c> toward zero.
///   2. Clamp the offset at zero to prevent overshooting.
///   3. Detect tile arrival and set <c>MovementState = Stopped</c>.
///   4. Write the final world-pixel position into <c>TransformComponent</c>.
///   5. Update <c>CharacterStateComponent.IsMoving / Direction</c> for the animation controller.
/// </summary>
internal sealed class MovementSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<MovementComponent, TransformComponent, CharacterStateComponent>();

    public override void Update(in float dt)
    {
        var deltaTime = dt;

        World.Query(in _query,
            (ref MovementComponent movement,
             ref TransformComponent transform,
             ref CharacterStateComponent state) =>
            {
                Step(ref movement, deltaTime);

                // Round to integer pixels for crisp rendering; no sub-pixel texture sampling.
                transform.X = (int)(movement.TileX * Grid + movement.OffsetX);
                transform.Y = (int)(movement.TileY * Grid + movement.OffsetY);

                state.IsMoving = movement.OffsetX != 0f || movement.OffsetY != 0f;
            });
    }

    /// <summary>
    /// Advances the offset by <c>speed × dt</c> toward zero and detects tile arrival.
    /// </summary>
    private static void Step(ref MovementComponent m, float dt)
    {
        if (m.MovementState == MovementState.Stopped)
        {
            m.OffsetX = 0f;
            m.OffsetY = 0f;
            return;
        }

        float delta = m.SpeedPixelsPerSecond * dt;
        float prevX = m.OffsetX, prevY = m.OffsetY;

        switch (m.Direction)
        {
            case Direction.Up: m.OffsetY -= delta; break;
            case Direction.Down: m.OffsetY += delta; break;
            case Direction.Right: m.OffsetX += delta; break;
            case Direction.Left: m.OffsetX -= delta; break;
        }

        // Clamp at zero to prevent overshooting when delta is large (e.g. first frame spike).
        if (prevX > 0f && m.OffsetX < 0f) m.OffsetX = 0f;
        if (prevX < 0f && m.OffsetX > 0f) m.OffsetX = 0f;
        if (prevY > 0f && m.OffsetY < 0f) m.OffsetY = 0f;
        if (prevY < 0f && m.OffsetY > 0f) m.OffsetY = 0f;

        // Deadzone: snap sub-pixel residuals to zero so IsMoving becomes false
        // cleanly and the transform is never rendered at a fractional stale offset.
        if (MathF.Abs(m.OffsetX) < 0.1f) m.OffsetX = 0f;
        if (MathF.Abs(m.OffsetY) < 0.1f) m.OffsetY = 0f;

        // Arrival: offset consumed in the travel direction.
        var arrived = m.Direction switch
        {
            Direction.Right => m.OffsetX >= 0f,
            Direction.Left => m.OffsetX <= 0f,
            Direction.Down => m.OffsetY >= 0f,
            Direction.Up => m.OffsetY <= 0f,
            _ => true
        };

        if (arrived)
            m.MovementState = MovementState.Stopped;
    }
}
