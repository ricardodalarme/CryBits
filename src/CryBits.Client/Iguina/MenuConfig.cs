using System.Text.Json.Serialization;

namespace CryBits.Client.Iguina;

public sealed class MenuConfig
{
    [JsonPropertyName("background")]
    public PanelElement Background { get; set; } = new();

    [JsonPropertyName("optionsButton")]
    public Element OptionsButton { get; set; } = new();

    [JsonPropertyName("screens")]
    public List<ScreenData> Screens { get; set; } = [];
}

public sealed class ScreenData
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("panel")]
    public PanelElement Panel { get; set; } = new();

    [JsonPropertyName("elements")]
    public List<Element> Elements { get; set; } = [];
}

public sealed class PanelElement
{
    [JsonPropertyName("texture")]
    public string Texture { get; set; } = string.Empty;

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }
}

public sealed class Element
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("texture")]
    public string Texture { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("masked")]
    public bool Masked { get; set; }

    [JsonPropertyName("maxWidth")]
    public int MaxWidth { get; set; }

    [JsonPropertyName("columns")]
    public int Columns { get; set; }

    [JsonPropertyName("rows")]
    public int Rows { get; set; }

    [JsonPropertyName("slotSize")]
    public int SlotSize { get; set; }

    [JsonPropertyName("padding")]
    public int SlotPadding { get; set; }

    [JsonPropertyName("checked")]
    public bool Checked { get; set; }

    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    [JsonPropertyName("srcX")]
    public int SrcX { get; set; }

    [JsonPropertyName("srcY")]
    public int SrcY { get; set; }

    [JsonPropertyName("srcW")]
    public int SrcW { get; set; }

    [JsonPropertyName("srcH")]
    public int SrcH { get; set; }

    [JsonPropertyName("fontSize")]
    public int FontSize { get; set; }

    [JsonPropertyName("textFillColor")]
    public string TextFillColor { get; set; } = string.Empty;

    [JsonPropertyName("textOutlineColor")]
    public string TextOutlineColor { get; set; } = string.Empty;

    [JsonPropertyName("textOutlineWidth")]
    public int TextOutlineWidth { get; set; }

    [JsonPropertyName("placeholder")]
    public string PlaceholderText { get; set; } = string.Empty;

    [JsonPropertyName("maxLength")]
    public int MaxLength { get; set; }

    [JsonPropertyName("paddingL")]
    public int PaddingLeft { get; set; }

    [JsonPropertyName("paddingR")]
    public int PaddingRight { get; set; }

    [JsonPropertyName("paddingT")]
    public int PaddingTop { get; set; }

    [JsonPropertyName("paddingB")]
    public int PaddingBottom { get; set; }

    [JsonPropertyName("children")]
    public List<Element> Children { get; set; } = [];
}
