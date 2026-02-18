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
        return (Direction)direction switch
        {
            Direction.Up => new Point(Grid / 2 - 4, 0),
            Direction.Down => new Point(Grid / 2 - 4, Grid - 9),
            Direction.Left => new Point(0, Grid / 2 - 4),
            Direction.Right => new Point(Grid - 9, Grid / 2 - 4),
            _ => new Point(0)
        };
    }
}