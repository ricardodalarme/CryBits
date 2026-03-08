using CryBits.Enums;
using SFML.System;
using static CryBits.Globals;

namespace CryBits.Client.Framework.Entities.Tile;

[Serializable]
public class Tile
{
    /// <summary>Cached tile metadata arrays, indexed by tileset.</summary>
    public static Tile[] List = [];

    /// <summary>Number of tiles horizontally.</summary>
    public byte Width { get; set; }

    /// <summary>Number of tiles vertically.</summary>
    public byte Height { get; set; }

    /// <summary>Tile metadata grid for this tileset.</summary>
    public TileData[,] Data { get; set; }

    public Tile(Vector2i textureSize)
    {
        var width = textureSize.X / Grid - 1;
        var height = textureSize.Y / Grid - 1;

        // Resize fields
        Width = (byte)width;
        Height = (byte)height;
        Data = new TileData[width + 1, height + 1];

        for (byte x = 0; x <= width; x++)
            for (byte y = 0; y <= height; y++)
                Data[x, y] = new TileData
                {
                    Block = new bool[(byte)Direction.Count]
                };
    }
}
