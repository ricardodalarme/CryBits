using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Entities;

internal abstract class Character
{
    /// <summary>Core character state and position fields.</summary>
    public short[] Vital = new short[(byte)Enums.Vital.Count];

    public byte X;
    public byte Y;
    public Direction Direction;
    public Movement Movement;
    public short X2;
    public short Y2;
    public byte Animation;
    public bool Attacking;
    public int Hurt;
    public int AttackTimer;

    /// <summary>Exact X position in pixels.</summary>
    public int PixelX => X * Grid + X2;

    /// <summary>Exact Y position in pixels.</summary>
    public int PixelY => Y * Grid + Y2;

    protected void ProcessMovement()
    {
        byte speed = 0;
        short x = X2, y = Y2;

        // Reset animation if stopped
        if (Animation == AnimationStopped) Animation = AnimationRight;

        // Determine movement speed
        switch (Movement)
        {
            case Movement.Walking: speed = 2; break;
            case Movement.Moving: speed = 3; break;
            case Movement.Stopped:
                // Reset offsets
                X2 = 0;
                Y2 = 0;
                return;
        }

        // Apply movement to pixel offsets
        switch (Direction)
        {
            case Direction.Up: Y2 -= speed; break;
            case Direction.Down: Y2 += speed; break;
            case Direction.Right: X2 += speed; break;
            case Direction.Left: X2 -= speed; break;
        }

        // Clamp offsets to avoid overshoot
        if (x > 0 && X2 < 0) X2 = 0;
        if (x < 0 && X2 > 0) X2 = 0;
        if (y > 0 && Y2 < 0) Y2 = 0;
        if (y < 0 && Y2 > 0) Y2 = 0;

        // Only change animation frame when movement finishes and offsets are zero
        if (Direction == Direction.Right || Direction == Direction.Down)
        {
            if (X2 < 0 || Y2 < 0)
                return;
        }
        else if (X2 > 0 || Y2 > 0)
            return;

        Movement = Movement.Stopped;
        if (Animation == AnimationLeft)
            Animation = AnimationRight;
        else
            Animation = AnimationLeft;
    }
}
