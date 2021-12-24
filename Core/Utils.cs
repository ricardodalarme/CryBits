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
        switch (direction)
        {
            case Direction.Up: return Direction.Down;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: return Direction.Count;
        }
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