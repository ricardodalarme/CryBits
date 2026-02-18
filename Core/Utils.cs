using System;
using CryBits.Enums;

namespace CryBits;

public static class Utils
{
    // Números aleatórios
    public static readonly Random MyRandom = new();

    public static Direction ReverseDirection(Direction direction)
    {
        // Retorna a direção inversa
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.Count
        };
    }

    public static void NextTile(Direction direction, ref byte x, ref byte y)
    {
        // Próximo azulejo
        switch (direction)
        {
            case Direction.Up: y--; break;
            case Direction.Down: y++; break;
            case Direction.Right: x++; break;
            case Direction.Left: x--; break;
        }
    }
}