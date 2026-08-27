using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Persistence.Serialization;
using System.Drawing;
using System.Text.Json;

namespace CryBits.Client.Framework.Persistence.Repositories;

public static class TileRepository
{
    public static Tile[] ReadAll()
    {
        var list = new Tile[Textures.Tiles.Count];
        for (byte i = 1; i < list.Length; i++) list[i] = Read(i);
        return list;
    }

    private static Tile Read(byte index)
    {
        var path = Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format;

        if (!File.Exists(path))
        {
            var texture = Textures.Tiles[index]!;
            var tile = new Tile(new Size(texture.Width, texture.Height));
            Write(index, tile);
            return tile;
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Tile>(json, JsonConfig.Options)!;
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
        var path = Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format;
        var json = JsonSerializer.Serialize(tile, JsonConfig.Options);
        File.WriteAllText(path, json);
    }
}
