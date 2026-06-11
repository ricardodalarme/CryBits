using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Enums;
using System.Text.Json.Serialization;

namespace CryBits.Client.Framework.Persistence.Dtos;

/// <summary>Root DTO – one per Tools.json file.</summary>
public sealed class ToolsJsonRoot
{
    public List<ScreenDto> Screens { get; set; } = [];
}

public sealed class ScreenDto
{
    public string Name { get; set; } = string.Empty;
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
    public string Name { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public bool Visible { get; set; }
    public List<ComponentDto> Children { get; set; } = [];
}

public sealed class LabelDto : ComponentDto
{
    public string Text { get; set; } = string.Empty;
    public int Color { get; set; } = 0xFFFFFF;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextAlign Alignment { get; set; } = TextAlign.Left;
    public int MaxWidth { get; set; } = 0;
}

public sealed class ButtonDto : ComponentDto
{
    public byte TextureNum { get; set; }
}

public sealed class TextBoxDto : ComponentDto
{
    public short MaxCharacters { get; set; }
    public short Width { get; set; }
    public bool Password { get; set; }
}

public sealed class PanelDto : ComponentDto
{
    public byte TextureNum { get; set; }
}

public sealed class CheckBoxDto : ComponentDto
{
    public string Text { get; set; } = string.Empty;
    public bool Checked { get; set; }
}

public sealed class ProgressBarDto : ComponentDto
{
    public int SourceY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public sealed class SlotGridDto : ComponentDto
{
    public byte Columns { get; set; } = 1;
    public byte SlotSize { get; set; } = 32;
    public byte Padding { get; set; } = 4;
    public byte Rows { get; set; } = 1;
}

public sealed class PictureDto : ComponentDto
{
    public int Width { get; set; }
    public int Height { get; set; }
}
