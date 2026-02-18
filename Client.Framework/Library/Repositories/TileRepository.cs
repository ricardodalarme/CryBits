using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;

namespace CryBits.Client.Framework.Library.Repositories;

public static class TileRepository
{
    public static void ReadAll()
    {
        // Lê os dados
        Tile.List = new Tile[Textures.Tiles.Count];
        for (byte i = 1; i < Tile.List.Length; i++) Read(i);
    }

    private static void Read(byte index)
    {
        var file = new FileInfo(Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format);

        // Evita erros
        if (!file.Exists)
        {
            var textureSize = Textures.Tiles[index].ToSize();
            Tile.List[index] = new Tile(textureSize);
            Write(index);
            return;
        }

        // Lê os dados
        using var stream = file.OpenRead();
#pragma warning disable SYSLIB0011
        Tile.List[index] = (Tile)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }

    public static void WriteAll()
    {
        for (byte i = 1; i < Tile.List.Length; i++) Write(i);
    }

    public static void Write(byte index)
    {
        using var stream = new FileInfo(Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format)
            .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, Tile.List[index]);
#pragma warning restore SYSLIB0011
    }
}
