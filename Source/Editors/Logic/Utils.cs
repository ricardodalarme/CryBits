using System;
using System.Drawing;

namespace Logic
{
    static class Utils
    {
        // Dimensão das grades 
        public const byte Grid = 32;
        public static Size Grid_Size = new Size(Grid, Grid);

        // Números aleAmountatórios
        public static Random MyRandom = new Random();

        public static Point Block_Position(byte Direction)
        {
            // Retorna a posição de cada seta do bloqueio direcional
            switch ((Directions)Direction)
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