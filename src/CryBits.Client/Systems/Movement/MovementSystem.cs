using CryBits.Client.Components;
using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using static CryBits.Definitions.Globals;
using MovementState = CryBits.Definitions.Common.Movement;

namespace CryBits.Client.Systems.Movement;

internal sealed class MovementSystem(World world) : IClientSystem
{
    public void Update(float dt)
    {
        foreach (var state in world.All)
        {
            var movement = state.Get<MovementComponent>();
            var transform = state.Get<TransformComponent>();
            if (movement == null || transform == null) continue;

            if (!state.Has<LocalPlayerTag>())
            {
                var pos = state.Get<Position>();
                if (pos != null && (movement.TileX != pos.X || movement.TileY != pos.Y))
                {
                    var offsetX = pos.Direction switch
                    {
                        Direction.Right => -Grid,
                        Direction.Left => Grid,
                        _ => 0f
                    };
                    var offsetY = pos.Direction switch
                    {
                        Direction.Up => Grid,
                        Direction.Down => -Grid,
                        _ => 0f
                    };
                    movement = new MovementComponent(
                        pos.X, pos.Y, offsetX, offsetY,
                        movement.SpeedPixelsPerSecond, MovementState.Walking, pos.Direction
                    );
                    world.Set(state.Id, movement);
                }
            }

            var newMovement = Step(movement, dt);

            if (newMovement != movement)
                world.Set(state.Id, newMovement);

            world.Set(state.Id, new TransformComponent(
                (int)(newMovement.TileX * Grid + newMovement.OffsetX),
                (int)(newMovement.TileY * Grid + newMovement.OffsetY)
            ));
        }
    }

    private static MovementComponent Step(MovementComponent m, float dt)
    {
        if (m.MovementState == MovementState.Stopped)
            return m with { OffsetX = 0f, OffsetY = 0f };

        var delta = m.SpeedPixelsPerSecond * dt;
        float prevX = m.OffsetX, prevY = m.OffsetY;

        var newOffsetX = m.Direction switch
        {
            Direction.Right => m.OffsetX + delta,
            Direction.Left => m.OffsetX - delta,
            _ => m.OffsetX
        };
        var newOffsetY = m.Direction switch
        {
            Direction.Down => m.OffsetY + delta,
            Direction.Up => m.OffsetY - delta,
            _ => m.OffsetY
        };

        if (prevX > 0f && newOffsetX < 0f) newOffsetX = 0f;
        if (prevX < 0f && newOffsetX > 0f) newOffsetX = 0f;
        if (prevY > 0f && newOffsetY < 0f) newOffsetY = 0f;
        if (prevY < 0f && newOffsetY > 0f) newOffsetY = 0f;

        var clampedOffsetX = MathF.Abs(newOffsetX) < 0.1f ? 0f : newOffsetX;
        var clampedOffsetY = MathF.Abs(newOffsetY) < 0.1f ? 0f : newOffsetY;

        var arrived = m.Direction switch
        {
            Direction.Right => clampedOffsetX >= 0f,
            Direction.Left => clampedOffsetX <= 0f,
            Direction.Down => clampedOffsetY >= 0f,
            Direction.Up => clampedOffsetY <= 0f,
            _ => true
        };

        return m with
        {
            OffsetX = clampedOffsetX,
            OffsetY = clampedOffsetY,
            MovementState = arrived ? MovementState.Stopped : m.MovementState
        };
    }
}
