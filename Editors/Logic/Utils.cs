using System.Drawing;
using static CryBits.Defaults;

namespace CryBits.Editors.Logic
{
    internal static class Utils
    {
        // Dimensão das grades 
        public static Size GridSize = new Size(Grid, Grid);

        public static Point Block_Position(byte direction)
        {
            // Retorna a posição de cada seta do bloqueio direcional
            switch ((Directions)direction)
            {
                case Directions.Up: return new Point(Grid / 2 - 4, 0);
                case Directions.Down: return new Point(Grid / 2 - 4, Grid - 9);
                case Directions.Left: return new Point(0, Grid / 2 - 4);
                case Directions.Right: return new Point(Grid - 9, Grid / 2 - 4);
                default: return new Point(0);
            }
        }
    }
}