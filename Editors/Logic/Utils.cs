using System;
using System.Drawing;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Editors.Logic;

internal static class Utils
{
    // Dimensão das grades 
    public static Size GridSize = new(Grid, Grid);

    public static Point Block_Position(byte direction)
    {
        // Retorna a posição de cada seta do bloqueio direcional
        switch ((Direction)direction)
        {
            case Direction.Up: return new Point(Grid / 2 - 4, 0);
            case Direction.Down: return new Point(Grid / 2 - 4, Grid - 9);
            case Direction.Left: return new Point(0, Grid / 2 - 4);
            case Direction.Right: return new Point(Grid - 9, Grid / 2 - 4);
            default: return new Point(0);
        }
    }
}