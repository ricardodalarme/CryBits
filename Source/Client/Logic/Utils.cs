using System.Drawing;
using CryBits.Client.UI;
using SFML.Graphics;
using static CryBits.Utils;
using Graphics = CryBits.Client.Media.Graphics;

namespace CryBits.Client.Logic
{
    internal static class Utils
    {
        // Converte o valor em uma posição adequada à camera
        public static int ConvertX(int x) => x - (Camera.TileSight.X * Grid) - Camera.StartSight.X;
        public static int ConvertY(int y) => y - (Camera.TileSight.Y * Grid) - Camera.StartSight.Y;

        public static Directions ReverseDirection(Directions direction)
        {
            // Retorna a direção inversa
            switch (direction)
            {
                case Directions.Up: return Directions.Down;
                case Directions.Down: return Directions.Up;
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                default: return Directions.Count;
            }
        }

        public static void NextTile(Directions direction, ref byte x, ref byte y)
        {
            // Próximo azulejo
            switch (direction)
            {
                case Directions.Up: y -= 1; break;
                case Directions.Down: y += 1; break;
                case Directions.Right: x += 1; break;
                case Directions.Left: x -= 1; break;
            }
        }

        public static bool IsAbove(Rectangle rectangle)
        {
            // Verficia se o Window.Mouse está sobre o objeto
            if (Windows.Mouse.X >= rectangle.X && Windows.Mouse.X <= rectangle.X + rectangle.Width)
                if (Windows.Mouse.Y >= rectangle.Y && Windows.Mouse.Y <= rectangle.Y + rectangle.Height)
                    return true;

            // Se não, retornar um valor nulo
            return false;
        }

        public static short MeasureString(string text)
        {
            // Dados do texto
            Text tempText = new Text(text, Graphics.FontDefault);
            tempText.CharacterSize = 10;
            return (short)tempText.GetLocalBounds().Width;
        }

        public static string TextBreak(string text, int width)
        {
            // Previne sobrecargas
            if (string.IsNullOrEmpty(text)) return text;

            // Usado para fazer alguns calculosk
            int textWidth = MeasureString(text);

            // Diminui o tamanho do texto até que ele caiba no digitalizador
            while (textWidth - width >= 0)
            {
                text = text.Substring(1);
                textWidth = MeasureString(text);
            }

            return text;
        }

        public static byte Slot(Panels panel, byte offX, byte offY, byte lines, byte columns, byte grid = 32, byte gap = 4)
        {
            int size = grid + gap;
            Point start = panel.Position + new Size(offX, offY);
            Point slot = new Point((Windows.Mouse.X - start.X) / size, (Windows.Mouse.Y - start.Y) / size);

            // Verifica se o Window.Mouse está sobre o slot
            if (slot.Y < 0 || slot.X < 0 || slot.X >= columns || slot.Y >= lines) return 0;
            if (!IsAbove(new Rectangle(start.X + slot.X * size, start.Y + slot.Y * size, grid, grid))) return 0;
            if (!panel.Visible) return 0;

            // Retorna o slot
            return (byte)(slot.Y * columns + slot.X + 1);
        }
    }
}