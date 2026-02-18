using System.Text.Json;
using System.Text.Json.Serialization;
using CryBits.Client.Framework.Interfacily.Enums;

namespace CryBits.Client.Framework.Library;

/// <summary>Shared JSON serializer options used by all read/write operations.</summary>
public static class JsonConfig
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
}

// ─── DTOs that represent the on-disk JSON layout ────────────────────────────

/// <summary>Root DTO – one per Tools.json file.</summary>
public sealed class ToolsJsonRoot
{
    [JsonPropertyName("screens")]
    public List<ScreenDto> Screens { get; set; } = new();
}

public sealed class ScreenDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("children")]
    public List<ComponentDto> Children { get; set; } = new();
}

/// <summary>
/// Polymorphic component DTO.
/// The "type" discriminator matches <see cref="ToolType"/> names.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ButtonDto), "Button")]
[JsonDerivedType(typeof(TextBoxDto), "TextBox")]
[JsonDerivedType(typeof(PanelDto), "Panel")]
[JsonDerivedType(typeof(CheckBoxDto), "CheckBox")]
public abstract class ComponentDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("visible")]
    public bool Visible { get; set; }

    [JsonPropertyName("children")]
    public List<ComponentDto> Children { get; set; } = new();
}

public sealed class ButtonDto : ComponentDto
{
    [JsonPropertyName("textureNum")]
    public byte TextureNum { get; set; }
}

public sealed class TextBoxDto : ComponentDto
{
    [JsonPropertyName("maxCharacters")]
    public short MaxCharacters { get; set; }

    [JsonPropertyName("width")]
    public short Width { get; set; }

    [JsonPropertyName("password")]
    public bool Password { get; set; }
}

public sealed class PanelDto : ComponentDto
{
    [JsonPropertyName("textureNum")]
    public byte TextureNum { get; set; }
}

public sealed class CheckBoxDto : ComponentDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("checked")]
    public bool Checked { get; set; }
}

// ─── Options DTO ─────────────────────────────────────────────────────────────

public sealed class OptionsDto
{
    [JsonPropertyName("saveUsername")] public bool SaveUsername { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; } = string.Empty;
    [JsonPropertyName("sounds")] public bool Sounds { get; set; } = true;
    [JsonPropertyName("musics")] public bool Musics { get; set; } = true;
    [JsonPropertyName("chat")] public bool Chat { get; set; } = true;
    [JsonPropertyName("fps")] public bool Fps { get; set; }
    [JsonPropertyName("latency")] public bool Latency { get; set; }
    [JsonPropertyName("party")] public bool Party { get; set; } = true;
    [JsonPropertyName("trade")] public bool Trade { get; set; } = true;
    [JsonPropertyName("preMapGrid")] public bool PreMapGrid { get; set; }
    [JsonPropertyName("preMapView")] public bool PreMapView { get; set; }
    [JsonPropertyName("preMapAudio")] public bool PreMapAudio { get; set; }
}
