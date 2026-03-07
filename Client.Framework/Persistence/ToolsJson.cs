using System.Text.Json;
using System.Text.Json.Serialization;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Enums;

namespace CryBits.Client.Framework.Persistence;

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
    public List<ScreenDto> Screens { get; set; } = [];
}

public sealed class ScreenDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("children")]
    public List<ComponentDto> Children { get; set; } = [];
}

/// <summary>
/// Polymorphic component DTO.
/// The "type" discriminator matches <see cref="ToolType"/> names.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(LabelDto), "Label")]
[JsonDerivedType(typeof(ButtonDto), "Button")]
[JsonDerivedType(typeof(TextBoxDto), "TextBox")]
[JsonDerivedType(typeof(PanelDto), "Panel")]
[JsonDerivedType(typeof(CheckBoxDto), "CheckBox")]
[JsonDerivedType(typeof(ProgressBarDto), "ProgressBar")]
[JsonDerivedType(typeof(SlotGridDto), "SlotGrid")]
[JsonDerivedType(typeof(PictureDto), "Picture")]
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
    public List<ComponentDto> Children { get; set; } = [];
}

public sealed class LabelDto : ComponentDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public int Color { get; set; } = 0xFFFFFF;

    [JsonPropertyName("alignment")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextAlign Alignment { get; set; } = TextAlign.Left;

    [JsonPropertyName("maxWidth")]
    public int MaxWidth { get; set; } = 0;
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

public sealed class ProgressBarDto : ComponentDto
{
    [JsonPropertyName("sourceY")]
    public int SourceY { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}

public sealed class SlotGridDto : ComponentDto
{
    [JsonPropertyName("columns")]
    public byte Columns { get; set; } = 1;

    [JsonPropertyName("slotSize")]
    public byte SlotSize { get; set; } = 32;

    [JsonPropertyName("padding")]
    public byte Padding { get; set; } = 4;

    [JsonPropertyName("rows")]
    public byte Rows { get; set; } = 1;
}
public sealed class PictureDto : ComponentDto
{
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}
// ─── Keybindings DTO ─────────────────────────────────────────────────────────

/// <summary>
/// On-disk representation of <see cref="KeyBindings"/>.
/// Movement actions are stored as <see cref="SFML.Window.Keyboard.Scancode"/> names;
/// all other actions as <see cref="SFML.Window.Keyboard.Key"/> names.
/// </summary>
public sealed class KeyBindingsDto
{
    // Movement
    [JsonPropertyName("moveUp")]    public string MoveUp    { get; set; } = "Up";
    [JsonPropertyName("moveDown")]  public string MoveDown  { get; set; } = "Down";
    [JsonPropertyName("moveLeft")]  public string MoveLeft  { get; set; } = "Left";
    [JsonPropertyName("moveRight")] public string MoveRight { get; set; } = "Right";

    // Held actions
    [JsonPropertyName("run")]    public string Run    { get; set; } = "LShift";
    [JsonPropertyName("attack")] public string Attack { get; set; } = "LControl";

    // Discrete actions
    [JsonPropertyName("chat")]    public string Chat    { get; set; } = "Enter";
    [JsonPropertyName("collect")] public string Collect { get; set; } = "Space";
    [JsonPropertyName("hotbar1")] public string Hotbar1 { get; set; } = "Num1";
    [JsonPropertyName("hotbar2")] public string Hotbar2 { get; set; } = "Num2";
    [JsonPropertyName("hotbar3")] public string Hotbar3 { get; set; } = "Num3";
    [JsonPropertyName("hotbar4")] public string Hotbar4 { get; set; } = "Num4";
    [JsonPropertyName("hotbar5")] public string Hotbar5 { get; set; } = "Num5";
    [JsonPropertyName("hotbar6")] public string Hotbar6 { get; set; } = "Num6";
    [JsonPropertyName("hotbar7")] public string Hotbar7 { get; set; } = "Num7";
    [JsonPropertyName("hotbar8")] public string Hotbar8 { get; set; } = "Num8";
    [JsonPropertyName("hotbar9")] public string Hotbar9 { get; set; } = "Num9";
    [JsonPropertyName("hotbar0")] public string Hotbar0 { get; set; } = "Num0";
}

// ─── Options DTO ─────────────────────────────────────────────────────────────

public sealed class OptionsDto
{
    [JsonPropertyName("saveUsername")] public bool SaveUsername { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; } = string.Empty;
    [JsonPropertyName("sounds")] public bool Sounds { get; set; } = true;
    [JsonPropertyName("musics")] public bool Musics { get; set; } = true;
    [JsonPropertyName("chat")] public bool Chat { get; set; } = true;
    [JsonPropertyName("showMetrics")] public bool ShowMetrics { get; set; }
    [JsonPropertyName("party")] public bool Party { get; set; } = true;
    [JsonPropertyName("trade")] public bool Trade { get; set; } = true;
    [JsonPropertyName("preMapGrid")] public bool PreMapGrid { get; set; }
    [JsonPropertyName("preMapView")] public bool PreMapView { get; set; }
    [JsonPropertyName("preMapAudio")] public bool PreMapAudio { get; set; }
}
