using CryBits.Client.Entities;
using System.Drawing;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Logic
{
    static class Camera
    {
        // Visão do jogador
        public static Point Start_Sight;
        public static Rectangle Tile_Sight;

        public static void Update()
        {
            Point End = new Point(), Start = new Point(), Position = new Point();

            // Centro da tela
            Position.X = Player.Me.X2 + Grid;
            Position.Y = Player.Me.Y2 + Grid;

            // Início da tela
            Start.X = Player.Me.X - ((Map_Width + 1) / 2) - 1;
            Start.Y = Player.Me.Y - ((Map_Height + 1) / 2) - 1;

            // Reajusta a posição horizontal da tela
            if (Start.X < 0)
            {
                Position.X = 0;
                if (Start.X == -1 && Player.Me.X2 > 0) Position.X = Player.Me.X2;
                Start.X = 0;
            }

            // Reajusta a posição vertical da tela
            if (Start.Y < 0)
            {
                Position.Y = 0;
                if (Start.Y == -1 && Player.Me.Y2 > 0) Position.Y = Player.Me.Y2;
                Start.Y = 0;
            }

            // Final da tela
            End.X = Start.X + (Map_Width + 1) + 1;
            End.Y = Start.Y + (Map_Height + 1) + 1;

            // Reajusta a posição horizontal da tela
            if (End.X > Map_Width)
            {
                Position.X = Grid;
                if (End.X == Map_Width + 1 && Player.Me.X2 < 0) Position.X = Player.Me.X2 + Grid;
                End.X = Map_Width;
                Start.X = End.X - Map_Width - 1;
            }

            // Reajusta a posição vertical da tela
            if (End.Y > Map_Height)
            {
                Position.Y = Grid;
                if (End.Y == Map_Height + 1 && Player.Me.Y2 < 0) Position.Y = Player.Me.Y2 + Grid;
                End.Y = Map_Height;
                Start.Y = End.Y - Map_Height - 1;
            }

            // Define a dimensão dos azulejos vistos
            Tile_Sight.Y = Start.Y;
            Tile_Sight.Height = End.Y;
            Tile_Sight.X = Start.X;
            Tile_Sight.Width = End.X;

            // Define a posição da câmera
            Start_Sight.Y = Position.Y;
            Start_Sight.X = Position.X;
        }
    }
}