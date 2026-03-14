using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Client.Framework.Persistence.Repositories;

public static class TileRepository
{
    /// <summary>Read all tile metadata from disk.</summary>
    public static Tile[] ReadAll()
    {
        var list = new Tile[Textures.Tiles.Count];
        for (byte i = 1; i < list.Length; i++) list[i] = Read(i);
        return list;
    }

    private static Tile Read(byte index)
    {
        var file = new FileInfo(Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format);

        // If the tile file is missing, create default data to avoid read errors.
        if (!file.Exists)
        {
            var textureSize = Textures.Tiles[index].ToSize();
            var tile = new Tile(textureSize);
            Write(index, tile);
            return tile;
        }

        // Read data
        using var stream = file.OpenRead();
#pragma warning disable SYSLIB0011
        return (Tile)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }

    public static void WriteAll()
    {
        for (byte i = 1; i < Tile.List.Length; i++) Write(i, Tile.List[i]);
    }

    public static void Write(byte index)
    {
        Write(index, Tile.List[index]);
    }

    private static void Write(byte index, Tile tile)
    {
        using var stream = new FileInfo(Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format)
            .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, tile);
#pragma warning restore SYSLIB0011
    }
}
