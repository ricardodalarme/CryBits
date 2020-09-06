using Interface;
using System;
using System.Drawing;
using static Logic.Game;

namespace Logic
{
    static class Utils
    {
        // Números aleatórios
        public static Random MyRandom = new Random();

        // Converte o valor em uma posição adequada à camera
        public static int ConvertX(int x) => x - (Camera.Tile_Sight.X * Grid) - Camera.Start_Sight.X;
        public static int ConvertY(int y) => y - (Camera.Tile_Sight.Y * Grid) - Camera.Start_Sight.Y;

        public static Directions ReverseDirection(Directions Direction)
        {
            // Retorna a direção inversa
            switch (Direction)
            {
                case Directions.Up: return Directions.Down;
                case Directions.Down: return Directions.Up;
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                default: return Directions.Count;
            }
        }

        public static void NextTile(Directions Direction, ref byte X, ref byte Y)
        {
            // Próximo azulejo
            switch (Direction)
            {
                case Directions.Up: Y -= 1; break;
                case Directions.Down: Y += 1; break;
                case Directions.Right: X += 1; break;
                case Directions.Left: X -= 1; break;
            }
        }

        public static bool IsAbove(Rectangle Rectangle)
        {
            // Verficia se o Window.Mouse está sobre o objeto
            if (Windows.Mouse.X >= Rectangle.X && Windows.Mouse.X <= Rectangle.X + Rectangle.Width)
                if (Windows.Mouse.Y >= Rectangle.Y && Windows.Mouse.Y <= Rectangle.Y + Rectangle.Height)
                    return true;

            // Se não, retornar um valor nulo
            return false;
        }

        public static short MeasureString(string Text)
        {
            // Dados do texto
            SFML.Graphics.Text TempText = new SFML.Graphics.Text(Text, Graphics.Font_Default);
            TempText.CharacterSize = 10;
            return (short)TempText.GetLocalBounds().Width;
        }

        public static string TextBreak(string Text, int Width)
        {
            // Previne sobrecargas
            if (string.IsNullOrEmpty(Text)) return Text;

            // Usado para fazer alguns calculosk
            int Text_Width = MeasureString(Text);

            // Diminui o tamanho do texto até que ele caiba no digitalizador
            while (Text_Width - Width >= 0)
            {
                Text = Text.Substring(1);
                Text_Width = MeasureString(Text);
            }

            return Text;
        }

        public static byte Slot(Panels Panel, byte OffX, byte OffY, byte Lines, byte Columns, byte Grid = 32, byte Gap = 4)
        {
            int Size = Grid + Gap;
            Point Start = Panel.Position + new Size(OffX, OffY);
            Point Slot = new Point((Windows.Mouse.X - Start.X) / Size, (Windows.Mouse.Y - Start.Y) / Size);

            // Verifica se o Window.Mouse está sobre o slot
            if (Slot.Y < 0 || Slot.X < 0 || Slot.X >= Columns || Slot.Y >= Lines) return 0;
            if (!IsAbove(new Rectangle(Start.X + Slot.X * Size, Start.Y + Slot.Y * Size, Grid, Grid))) return 0;
            if (!Panel.Visible) return 0;

            // Retorna o slot
            return (byte)(Slot.Y * Columns + Slot.X + 1);
        }
    }
}