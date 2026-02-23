using CryBits.Client.ECS.Components;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Advances the pixel-offset interpolation for every entity that is currently
/// moving between tiles (<see cref="MovementComponent.Current"/> != Stopped).
///
/// Logic ported from the old <c>Character.ProcessMovement()</c> but operating
/// on components instead of object fields, making it fully stateless and
/// independently testable.
/// </summary>
internal sealed class MovementSystem : IUpdateSystem
{
    public void Update(GameContext ctx)
    {
        foreach (var (id, transform, movement) in ctx.World.Query<TransformComponent, MovementComponent>())
        {
            ctx.World.TryGet<AnimationComponent>(id, out var animation);
            ProcessMovement(transform, movement, animation);
        }
    }

    private static void ProcessMovement(
        TransformComponent transform,
        MovementComponent movement,
        AnimationComponent? animation)
    {
        // Stopped: ensure offsets are zeroed and exit early.
        if (movement.Current == Movement.Stopped)
        {
            transform.PixelOffsetX = 0;
            transform.PixelOffsetY = 0;
            return;
        }

        // If the animation was frozen on the stopped frame, restart walking.
        if (animation != null && animation.Frame == AnimationStopped)
            animation.Frame = AnimationRight;

        byte speed = movement.Current == Movement.Moving ? (byte)3 : (byte)2;

        var prevOffsetX = transform.PixelOffsetX;
        var prevOffsetY = transform.PixelOffsetY;

        // Advance the offset toward 0 each tick.
        switch (transform.Direction)
        {
            case Direction.Up: transform.PixelOffsetY -= speed; break;
            case Direction.Down: transform.PixelOffsetY += speed; break;
            case Direction.Right: transform.PixelOffsetX += speed; break;
            case Direction.Left: transform.PixelOffsetX -= speed; break;
        }

        // Clamp to prevent overshoot past zero.
        if (prevOffsetX > 0 && transform.PixelOffsetX < 0) transform.PixelOffsetX = 0;
        if (prevOffsetX < 0 && transform.PixelOffsetX > 0) transform.PixelOffsetX = 0;
        if (prevOffsetY > 0 && transform.PixelOffsetY < 0) transform.PixelOffsetY = 0;
        if (prevOffsetY < 0 && transform.PixelOffsetY > 0) transform.PixelOffsetY = 0;

        // Determine whether the entity has fully arrived at the destination tile.
        bool arrived = transform.Direction is Direction.Right or Direction.Down
            ? transform.PixelOffsetX >= 0 && transform.PixelOffsetY >= 0
            : transform.PixelOffsetX <= 0 && transform.PixelOffsetY <= 0;

        if (!arrived) return;

        movement.Current = Movement.Stopped;

        if (animation != null)
            animation.Frame = animation.Frame == AnimationLeft ? AnimationRight : AnimationLeft;
    }
}
