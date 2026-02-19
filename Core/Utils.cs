using System;
using CryBits.Enums;

namespace CryBits;

public static class Utils
{
    /// <summary>Shared <see cref="Random"/> instance used across the project.</summary>
    public static readonly Random MyRandom = new();

    /// <summary>Return the opposite of the specified direction.</summary>
    public static Direction ReverseDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.Count
        };
    }

    /// <summary>Advance the (x,y) coordinates by one tile in the given direction.</summary>
    /// <param name="direction">Direction to move.</param>
    /// <param name="x">X coordinate to modify.</param>
    /// <param name="y">Y coordinate to modify.</param>
    public static void NextTile(Direction direction, ref byte x, ref byte y)
    {
        switch (direction)
        {
            case Direction.Up: y--; break;
            case Direction.Down: y++; break;
            case Direction.Right: x++; break;
            case Direction.Left: x--; break;
        }
    }
}