using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Definitions.Common;
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

            Step(movement, dt);

            transform.X = (int)(movement.TileX * Grid + movement.OffsetX);
            transform.Y = (int)(movement.TileY * Grid + movement.OffsetY);
        }
    }

    private static void Step(MovementComponent m, float dt)
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

        if (prevX > 0f && m.OffsetX < 0f) m.OffsetX = 0f;
        if (prevX < 0f && m.OffsetX > 0f) m.OffsetX = 0f;
        if (prevY > 0f && m.OffsetY < 0f) m.OffsetY = 0f;
        if (prevY < 0f && m.OffsetY > 0f) m.OffsetY = 0f;

        if (MathF.Abs(m.OffsetX) < 0.1f) m.OffsetX = 0f;
        if (MathF.Abs(m.OffsetY) < 0.1f) m.OffsetY = 0f;

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
