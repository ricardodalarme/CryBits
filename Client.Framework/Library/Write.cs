using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Entities.Map;

namespace CryBits.Client.Framework.Library;

public static class Write
{
    public static void Options()
    {
        // Escreve as configurações
        using var data = new BinaryWriter(Directories.Options.OpenWrite());
        data.Write(Framework.Options.SaveUsername);
        data.Write(Framework.Options.Username);
        data.Write(Framework.Options.Sounds);
        data.Write(Framework.Options.Musics);
        data.Write(Framework.Options.Chat);
        data.Write(Framework.Options.Fps);
        data.Write(Framework.Options.Latency);
        data.Write(Framework.Options.Party);
        data.Write(Framework.Options.Trade);
        data.Write(Framework.Options.PreMapGrid);
        data.Write(Framework.Options.PreMapView);
        data.Write(Framework.Options.PreMapAudio);
    }

    public static void Button(BinaryWriter data, Button tool)
    {
        // Escreve os dados
        data.Write(tool.Name);
        data.Write(tool.Position.X);
        data.Write(tool.Position.Y);
        data.Write(tool.Visible);
        data.Write(tool.TextureNum);
    }

    public static void TextBox(BinaryWriter data, TextBox tool)
    {
        // Escreve os dados
        data.Write(tool.Name);
        data.Write(tool.Position.X);
        data.Write(tool.Position.Y);
        data.Write(tool.Visible);
        data.Write(tool.MaxCharacters);
        data.Write(tool.Width);
        data.Write(tool.Password);
    }

    public static void Panel(BinaryWriter data, Panel tool)
    {
        // Escreve os dados
        data.Write(tool.Name);
        data.Write(tool.Position.X);
        data.Write(tool.Position.Y);
        data.Write(tool.Visible);
        data.Write(tool.TextureNum);
    }

    public static void CheckBox(BinaryWriter data, CheckBox tool)
    {
        // Escreve os dados
        data.Write(tool.Name);
        data.Write(tool.Position.X);
        data.Write(tool.Position.Y);
        data.Write(tool.Visible);
        data.Write(tool.Text);
        data.Write(tool.Checked);
    }

    public static void Tiles()
    {
        // Escreve os dados
        for (byte i = 1; i < Entities.Tile.Tile.List.Length; i++) Tile(i);
    }

    public static void Tile(byte index)
    {
        // Escreve os dados
        using var stream = new FileInfo(Directories.Tiles.FullName + index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(stream, Entities.Tile.Tile.List[index]);
    }

    public static void Map(Map map)
    {
        // Escreve os dados
        using var stream = new FileInfo(Directories.MapsData.FullName + map.Id + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(stream, map);
    }
}