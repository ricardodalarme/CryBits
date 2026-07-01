using System.Text.Json.Serialization;

namespace CryBits.Client.Framework.Persistence.Dtos;

public sealed class UILayout
{
    public List<PanelElement> Screens { get; set; } = [];
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(PanelElement), "Panel")]
[JsonDerivedType(typeof(LabelElement), "Label")]
[JsonDerivedType(typeof(TitleElement), "Title")]
[JsonDerivedType(typeof(ParagraphElement), "Paragraph")]
[JsonDerivedType(typeof(ButtonElement), "Button")]
[JsonDerivedType(typeof(CheckboxElement), "Checkbox")]
[JsonDerivedType(typeof(RadioButtonElement), "RadioButton")]
[JsonDerivedType(typeof(TextInputElement), "TextInput")]
[JsonDerivedType(typeof(NumericInputElement), "NumericInput")]
[JsonDerivedType(typeof(ProgressBarElement), "ProgressBar")]
[JsonDerivedType(typeof(SliderElement), "Slider")]
[JsonDerivedType(typeof(PictureElement), "Picture")]
[JsonDerivedType(typeof(SlotGridElement), "SlotGrid")]
[JsonDerivedType(typeof(ListBoxElement), "ListBox")]
[JsonDerivedType(typeof(DropDownElement), "DropDown")]
public abstract class Element
{
    public string Name { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Visible { get; set; } = true;
    public string Anchor { get; set; } = "TopLeft";
    public string DraggableMode { get; set; } = "NotDraggable";
    public List<Element> Children { get; set; } = [];
}

public sealed class PanelElement : Element
{
    public string Texture { get; set; } = string.Empty;
}

public sealed class LabelElement : Element
{
    public string Text { get; set; } = string.Empty;
    public int MaxWidth { get; set; }
}

public sealed class TitleElement : Element
{
    public string Text { get; set; } = string.Empty;
}

public sealed class ParagraphElement : Element
{
    public string Text { get; set; } = string.Empty;
    public bool EnableStyleCommands { get; set; }
}

public sealed class ButtonElement : Element
{
    public string Texture { get; set; } = string.Empty;
    public bool Checked { get; set; }
    public bool ToggleCheckOnClick { get; set; }
    public bool ExclusiveSelection { get; set; }
    public bool CanClickToUncheck { get; set; } = true;
}

public sealed class CheckboxElement : Element
{
    public string Text { get; set; } = string.Empty;
    public bool Checked { get; set; }
    public bool ExclusiveSelection { get; set; }
}

public sealed class RadioButtonElement : Element
{
    public string Text { get; set; } = string.Empty;
    public bool Checked { get; set; }
}

public class TextInputElement : Element
{
    public string PlaceholderText { get; set; } = string.Empty;
    public int MaxLength { get; set; }
    public bool Masked { get; set; }
}

public sealed class NumericInputElement : TextInputElement
{
    public double DefaultValue { get; set; }
    public bool AcceptsDecimal { get; set; } = true;
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double ButtonsStepSize { get; set; } = 1;
}

public sealed class ProgressBarElement : Element
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; } = 100;
    public int Value { get; set; }
}

public sealed class SliderElement : Element
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; } = 10;
    public int Value { get; set; }
    public int StepsCount { get; set; }
    public string Orientation { get; set; } = "Horizontal";
    public int MouseWheelStep { get; set; } = 1;
    public bool FlippedDirection { get; set; }
}

public sealed class PictureElement : Element { }

public sealed class SlotGridElement : Element
{
    public int Columns { get; set; } = 1;
    public int Rows { get; set; } = 1;
    public int SlotSize { get; set; } = 32;
    public int Spacing { get; set; } = 4;
}

public class ListBoxElement : Element
{
    public List<string> Items { get; set; } = [];
    public int SelectedIndex { get; set; } = -1;
    public bool AllowDeselect { get; set; } = true;
}

public sealed class DropDownElement : ListBoxElement { }
