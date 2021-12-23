using System;
using System.Drawing;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Entities.Tile;

[Serializable]
public class Tile
{
    // Lista de dados
    public static Tile[] List;

    // Dados
    public byte Width { get; set; }
    public byte Height { get; set; }
    public TileData[,] Data { get; set; }

    public Tile(Size textureSize)
    {
        var size = new Size((textureSize.Width / Grid) - 1, (textureSize.Height / Grid) - 1);

        // Redimensiona os valores
        Width = (byte) size.Width;
        Height = (byte) size.Height;
        Data = new TileData[size.Width + 1, size.Height + 1];

        for (byte x = 0; x <= size.Width; x++)
        for (byte y = 0; y <= size.Height; y++)
            Data[x, y] = new TileData
            {
                Block = new bool[(byte) Direction.Count]
            };
    }
}