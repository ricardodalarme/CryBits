using System.Drawing;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Editors.Logic;

internal static class Utils
{
    /// <summary>Editor grid size in pixels.</summary>
    public static Size GridSize = new(Grid, Grid);

    /// <summary>Return the pixel offset for the directional block indicator.</summary>
    /// <param name="direction">Direction value (see <see cref="Direction"/>).</param>
    public static Point Block_Position(byte direction)
    {
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
