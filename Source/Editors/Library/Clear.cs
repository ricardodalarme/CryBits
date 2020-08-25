using Entities;
using System.Drawing;
using static Logic.Utils;

namespace Library
{
    static class Clear
    {
        public static void Tile(byte Index)
        {
            Size Texture_Size = Graphics.TSize(Graphics.Tex_Tile[Index]);
            Size Size = new Size(Texture_Size.Width / Grid - 1, Texture_Size.Height / Grid - 1);

            // Redimensiona os valores
            Lists.Tile[Index] = new Tile();
            Lists.Tile[Index].Width = (byte)Size.Width;
            Lists.Tile[Index].Height = (byte)Size.Height;
            Lists.Tile[Index].Data = new Tile_Data[Size.Width + 1, Size.Height + 1];

            for (byte x = 0; x <= Size.Width; x++)
                for (byte y = 0; y <= Size.Height; y++)
                {
                    Lists.Tile[Index].Data[x, y] = new Tile_Data();
                    Lists.Tile[Index].Data[x, y].Block = new bool[(byte)Directions.Count];
                }
        }
    }
}