using System.Drawing;
using CryBits.Client.UI;
using SFML.Graphics;
using static CryBits.Defaults;
using Graphics = CryBits.Client.Media.Graphics;

namespace CryBits.Client.Logic
{
    internal static class Utils
    {
        // Converte o valor em uma posição adequada à camera
        public static int ConvertX(int x) => x - Camera.TileSight.X * Grid - Camera.StartSight.X;
        public static int ConvertY(int y) => y - Camera.TileSight.Y * Grid - Camera.StartSight.Y;

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

        public static short Slot(Panels panel, byte offX, byte offY, byte lines, byte columns, byte grid = 32, byte gap = 4)
        {
            int size = grid + gap;
            Point start = panel.Position + new Size(offX, offY);
            Point slot = new Point((Windows.Mouse.X - start.X) / size, (Windows.Mouse.Y - start.Y) / size);

            // Verifica se o Window.Mouse está sobre o slot
            if (slot.Y < 0 || slot.X < 0 || slot.X >= columns || slot.Y >= lines) return -1;
            if (!IsAbove(new Rectangle(start.X + slot.X * size, start.Y + slot.Y * size, grid, grid))) return -1;
            if (!panel.Visible) return -1;

            // Retorna o slot
            return (short)(slot.Y * columns + slot.X);
        }
    }
}