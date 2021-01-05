using System;
using System.Drawing;
using CryBits.Editors.Media.Graphics;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Editors.Entities
{
    [Serializable]
    internal class Tile
    {
        // Lista de dados
        public static Tile[] List;

        // Dados
        public byte Width;
        public byte Height;
        public TileData[,] Data;

        public Tile(byte index)
        {
            Size textureSize = Textures.Tiles[index].ToSize();
            Size size = new Size((textureSize.Width / Grid) - 1, (textureSize.Height / Grid) - 1);

            // Redimensiona os valores
            Width = (byte)size.Width;
            Height = (byte)size.Height;
            Data = new TileData[size.Width + 1, size.Height + 1];

            for (byte x = 0; x <= size.Width; x++)
                for (byte y = 0; y <= size.Height; y++)
                    Data[x, y] = new TileData
                    {
                        Block = new bool[(byte)Direction.Count]
                    };
        }
    }

    [Serializable]
    internal class TileData
    {
        public byte Attribute;
        public bool[] Block = new bool[(byte)Direction.Count];
    }
}
