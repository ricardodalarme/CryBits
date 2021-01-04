using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Entities;
using static CryBits.Globals;

namespace CryBits.Client.Logic
{
    internal static class Camera
    {
        // Visão do jogador
        public static Point StartSight;
        public static Rectangle TileSight;

        public static void Update()
        {
            Point end = new(), start = new(), position = new();

            // Centro da tela
            position.X = Player.Me.X2 + Grid;
            position.Y = Player.Me.Y2 + Grid;

            // Início da tela
            start.X = Player.Me.X - (Map.Width + 1) / 2 - 1;
            start.Y = Player.Me.Y - (Map.Height + 1) / 2 - 1;

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
            end.X = start.X + Map.Width + 1 + 1;
            end.Y = start.Y + Map.Height + 1 + 1;

            // Reajusta a posição horizontal da tela
            if (end.X > Map.Width)
            {
                position.X = Grid;
                if (end.X == Map.Width + 1 && Player.Me.X2 < 0) position.X = Player.Me.X2 + Grid;
                end.X = Map.Width;
                start.X = end.X - Map.Width - 1;
            }

            // Reajusta a posição vertical da tela
            if (end.Y > Map.Height)
            {
                position.Y = Grid;
                if (end.Y == Map.Height + 1 && Player.Me.Y2 < 0) position.Y = Player.Me.Y2 + Grid;
                end.Y = Map.Height;
                start.Y = end.Y - Map.Height - 1;
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