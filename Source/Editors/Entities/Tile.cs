using Logic;
using System;
using System.Drawing;
using static Logic.Utils;

namespace Entities
{
    [Serializable]
    class Tile
    {
        public byte Width;
        public byte Height;
        public Tile_Data[,] Data;

        public Tile(byte Index)
        {
            Size Texture_Size = Graphics.TSize(Graphics.Tex_Tile[Index]);
            Size Size = new Size(Texture_Size.Width / Grid - 1, Texture_Size.Height / Grid - 1);

            // Redimensiona os valores
            Width = (byte)Size.Width;
            Height = (byte)Size.Height;
            Data = new Tile_Data[Size.Width + 1, Size.Height + 1];

            for (byte x = 0; x <= Size.Width; x++)
                for (byte y = 0; y <= Size.Height; y++)
                {
                    Data[x, y] = new Tile_Data();
                    Data[x, y].Block = new bool[(byte)Directions.Count];
                }
        }
    }

    [Serializable]
    class Tile_Data
    {
        public byte Attribute;
        public bool[] Block = new bool[(byte)Directions.Count];
    }
}
