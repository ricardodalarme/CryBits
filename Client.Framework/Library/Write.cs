using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Entities.Map;

namespace CryBits.Client.Framework.Library;

public static class Write
{
    // ─── Options ─────────────────────────────────────────────────────────────

    public static void Options()
    {
        // Escreve as configurações em JSON
        Directories.Options.Directory?.Create();
        var opts = new OptionsDto
        {
            SaveUsername = Framework.Options.SaveUsername,
            Username = Framework.Options.Username,
            Sounds = Framework.Options.Sounds,
            Musics = Framework.Options.Musics,
            Chat = Framework.Options.Chat,
            Fps = Framework.Options.Fps,
            Latency = Framework.Options.Latency,
            Party = Framework.Options.Party,
            Trade = Framework.Options.Trade,
            PreMapGrid = Framework.Options.PreMapGrid,
            PreMapView = Framework.Options.PreMapView,
            PreMapAudio = Framework.Options.PreMapAudio
        };
        using var stream = Directories.Options.Open(FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(stream, opts, JsonConfig.Options);
    }

    // ─── Interface (Tools) – helpers used by the Editors project ─────────────

    /// <summary>Converts a <see cref="Button"/> to its DTO form.</summary>
    public static ButtonDto ButtonDto(Button tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        TextureNum = tool.TextureNum
    };

    public static TextBoxDto TextBoxDto(TextBox tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        MaxCharacters = tool.MaxCharacters,
        Width = tool.Width,
        Password = tool.Password
    };

    public static PanelDto PanelDto(Panel tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        TextureNum = tool.TextureNum
    };

    public static CheckBoxDto CheckBoxDto(CheckBox tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        Text = tool.Text,
        Checked = tool.Checked
    };

    // ─── Tiles (keep BinaryFormatter – tile data is internal build output) ───

    public static void Tiles()
    {
        for (byte i = 1; i < Entities.Tile.Tile.List.Length; i++) Tile(i);
    }

    public static void Tile(byte index)
    {
        using var stream = new FileInfo(Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format).OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, Entities.Tile.Tile.List[index]);
#pragma warning restore SYSLIB0011
    }

    // ─── Maps (keep BinaryFormatter – maps are sent over the network) ─────────

    public static void Map(Map map)
    {
        using var stream = new FileInfo(Path.Combine(Directories.MapsData.FullName, map.Id.ToString()) + Directories.Format).OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, map);
#pragma warning restore SYSLIB0011
    }
}
