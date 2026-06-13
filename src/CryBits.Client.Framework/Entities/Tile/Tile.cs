using CryBits.Definitions.Common;
using CryBits.Definitions.Utils;
using System.Drawing;
using System.Text.Json.Serialization;
using static CryBits.Definitions.Globals;

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
    [JsonConverter(typeof(Array2DConverter<TileData>))]
    public TileData[,] Data { get; set; }

    public Tile(Size textureSize)
    {
        var size = new Size(textureSize.Width / Grid - 1, textureSize.Height / Grid - 1);

        // Resize fields
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
