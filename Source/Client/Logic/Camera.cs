using System.Drawing;
using CryBits.Client.Entities;
using static CryBits.Client.Logic.Game;
using static CryBits.Utils;

namespace CryBits.Client.Logic
{
    internal static class Camera
    {
        // Visão do jogador
        public static Point StartSight;
        public static Rectangle TileSight;

        public static void Update()
        {
            Point end = new Point(), start = new Point(), position = new Point();

            // Centro da tela
            position.X = Player.Me.X2 + Grid;
            position.Y = Player.Me.Y2 + Grid;

            // Início da tela
            start.X = Player.Me.X - ((MapWidth + 1) / 2) - 1;
            start.Y = Player.Me.Y - ((MapHeight + 1) / 2) - 1;

            // Reajusta a posição horizontal da tela
            if (start.X < 0)
            {
                position.X = 0;
                if (start.X == -1 && Player.Me.X2 > 0) position.X = Player.Me.X2;
                start.X = 0;
            }

            // Reajusta a posição vertical da tela
            if (start.Y < 0)
            {
                position.Y = 0;
                if (start.Y == -1 && Player.Me.Y2 > 0) position.Y = Player.Me.Y2;
                start.Y = 0;
            }

            // Final da tela
            end.X = start.X + (MapWidth + 1) + 1;
            end.Y = start.Y + (MapHeight + 1) + 1;

            // Reajusta a posição horizontal da tela
            if (end.X > MapWidth)
            {
                position.X = Grid;
                if (end.X == MapWidth + 1 && Player.Me.X2 < 0) position.X = Player.Me.X2 + Grid;
                end.X = MapWidth;
                start.X = end.X - MapWidth - 1;
            }

            // Reajusta a posição vertical da tela
            if (end.Y > MapHeight)
            {
                position.Y = Grid;
                if (end.Y == MapHeight + 1 && Player.Me.Y2 < 0) position.Y = Player.Me.Y2 + Grid;
                end.Y = MapHeight;
                start.Y = end.Y - MapHeight - 1;
            }

            // Define a dimensão dos azulejos vistos
            TileSight.Y = start.Y;
            TileSight.Height = end.Y;
            TileSight.X = start.X;
            TileSight.Width = end.X;

            // Define a posição da câmera
            StartSight.Y = position.Y;
            StartSight.X = position.X;
        }
    }
}