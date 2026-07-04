using CryBits.Client.Framework.Persistence.Dtos;
using CryBits.Client.Framework.UI.Entities;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;

namespace CryBits.Client.Framework.UI;

public static class LayoutBuilder
{
    private const string DefaultSlotGridTexture = "Textures/TextBox.png";

    private static readonly Dictionary<Type, Func<UISystem, Element, Entity>> Builders = new()
    {
        [typeof(PanelElement)] = (ui, e) => BuildPanel(ui, (PanelElement)e),
        [typeof(ButtonElement)] = (ui, e) => BuildButton(ui, (ButtonElement)e),
        [typeof(CheckboxElement)] = (ui, e) => BuildCheckbox(ui, (CheckboxElement)e),
        [typeof(RadioButtonElement)] = (ui, e) => BuildRadioButton(ui, (RadioButtonElement)e),
        [typeof(LabelElement)] = (ui, e) => BuildLabel(ui, (LabelElement)e),
        [typeof(TitleElement)] = (ui, e) => BuildTitle(ui, (TitleElement)e),
        [typeof(ParagraphElement)] = (ui, e) => BuildParagraph(ui, (ParagraphElement)e),
        [typeof(TextInputElement)] = (ui, e) => BuildTextInput(ui, (TextInputElement)e),
        [typeof(NumericInputElement)] = (ui, e) => BuildNumericInput(ui, (NumericInputElement)e),
        [typeof(ProgressBarElement)] = (ui, e) => BuildProgressBar(ui, (ProgressBarElement)e),
        [typeof(SliderElement)] = (ui, e) => BuildSlider(ui, (SliderElement)e),
        [typeof(PictureElement)] = (ui, e) => BuildPicture(ui, (PictureElement)e),
        [typeof(SlotGridElement)] = (ui, e) => BuildSlotGrid(ui, (SlotGridElement)e),
        [typeof(ListBoxElement)] = (ui, e) => BuildListBox(ui, (ListBoxElement)e),
        [typeof(DropDownElement)] = (ui, e) => BuildDropDown(ui, (DropDownElement)e)
    };

    public static (Panel panel, Dictionary<string, Entity> registry) BuildScreen(
        UISystem ui, PanelElement screenElement)
    {
        var registry = new Dictionary<string, Entity>();
        var panel = BuildPanel(ui, screenElement);

        foreach (var el in screenElement.Children)
            BuildElement(ui, el, panel, registry);

        return (panel, registry);
    }

    private static void ApplyAnchor(Entity entity, string anchorName)
    {
        if (Enum.TryParse<Anchor>(anchorName, out var anchor))
            entity.Anchor = anchor;
        else
            entity.Anchor = Anchor.TopLeft;
    }

    private static Entity? BuildElement(UISystem ui, Element el, Panel parent, Dictionary<string, Entity>? registry = null)
    {
        if (!Builders.TryGetValue(el.GetType(), out var factory))
            return null;

        var entity = factory(ui, el);
        registry?[el.Name] = entity;

        ApplyAnchor(entity, el.Anchor);
        entity.Offset.SetPixels(el.X, el.Y);
        entity.Visible = el.Visible;
        if (Enum.TryParse<DraggableMode>(el.DraggableMode, out var dragMode))
            entity.DraggableMode = dragMode;
        parent.AddChild(entity);

        if (el.Children.Count > 0 && entity is Panel entityPanel)
        {
            foreach (var child in el.Children)
                BuildElement(ui, child, entityPanel, registry);
        }

        return entity;
    }

    private static Button BuildButton(UISystem ui, ButtonElement el)
    {
        var btn = new Button(ui);
        if (!string.IsNullOrEmpty(el.Texture))
        {
            btn.OverrideStyles.Icon = new IconTexture
            {
                TextureId = el.Texture,
                SourceRect = new Rectangle { Width = el.Width, Height = el.Height },
                TextureScale = 1
            };
        }
        btn.Size.SetPixels(el.Width, el.Height);
        btn.Paragraph.Text = string.Empty;
        btn.ToggleCheckOnClick = el.ToggleCheckOnClick;
        btn.ExclusiveSelection = el.ExclusiveSelection;
        btn.CanClickToUncheck = el.CanClickToUncheck;
        btn.Checked = el.Checked;
        return btn;
    }

    private static TextInput BuildTextInput(UISystem ui, TextInputElement el)
    {
        var input = new TextInput(ui);
        input.Size.SetPixels(el.Width, el.Height);
        input.PlaceholderText = el.PlaceholderText;
        if (el.MaxLength > 0) input.MaxLength = el.MaxLength;
        if (el.Masked) input.MaskingCharacter = '*';
        return input;
    }

    private static NumericInput BuildNumericInput(UISystem ui, NumericInputElement el)
    {
        var input = new NumericInput(ui);
        input.Size.SetPixels(el.Width, el.Height);
        input.DefaultValue = (decimal)el.DefaultValue;
        input.AcceptsDecimal = el.AcceptsDecimal;
        if (el.MinValue != 0) input.MinValue = (decimal)el.MinValue;
        if (el.MaxValue != 0) input.MaxValue = (decimal)el.MaxValue;
        input.ButtonsStepSize = (decimal)el.ButtonsStepSize;
        return input;
    }

    private static Checkbox BuildCheckbox(UISystem ui, CheckboxElement el)
    {
        var cb = new Checkbox(ui) { Paragraph = { Text = el.Text, OverrideStyles = new StyleSheetState { Padding = new Sides { Left = 18 } } }, Checked = el.Checked, ExclusiveSelection = el.ExclusiveSelection };
        return cb;
    }

    private static RadioButton BuildRadioButton(UISystem ui, RadioButtonElement el)
    {
        var rb = new RadioButton(ui) { Paragraph = { Text = el.Text, OverrideStyles = new StyleSheetState { Padding = new Sides { Left = 18 } } }, Checked = el.Checked };
        return rb;
    }

    private static Label BuildLabel(UISystem ui, LabelElement el)
    {
        var label = new Label(ui) { Text = el.Text };
        if (el.MaxWidth > 0)
        {
            label.Size.SetPixels(el.MaxWidth, 20);
            label.TextOverflowMode = TextOverflowMode.WrapWords;
        }
        return label;
    }

    private static Title BuildTitle(UISystem ui, TitleElement el) => new(ui) { Text = el.Text };

    private static Paragraph BuildParagraph(UISystem ui, ParagraphElement el) =>
        new(ui) { Text = el.Text, EnableStyleCommands = el.EnableStyleCommands };

    private static Panel BuildPanel(UISystem ui, PanelElement el)
    {
        var panel = new Panel(ui);
        if (!string.IsNullOrEmpty(el.Texture))
        {
            panel.OverrideStyles.FillTextureStretched = new StretchedTexture
            {
                TextureId = el.Texture,
                SourceRect = new Rectangle { Width = Math.Max(el.Width, 1), Height = Math.Max(el.Height, 1) }
            };
        }
        panel.Size.SetPixels(el.Width, el.Height);
        return panel;
    }

    private static ProgressBar BuildProgressBar(UISystem ui, ProgressBarElement el)
    {
        var bar = new ProgressBar(ui);
        bar.Size.SetPixels(el.Width, el.Height);
        bar.MinValue = el.MinValue;
        bar.MaxValue = el.MaxValue;
        bar.ValueSafe = el.Value;
        return bar;
    }

    private static Slider BuildSlider(UISystem ui, SliderElement el)
    {
        var orientation = el.Orientation?.Equals("Vertical", StringComparison.OrdinalIgnoreCase) == true
            ? Orientation.Vertical : Orientation.Horizontal;
        var slider = new Slider(ui, orientation);
        slider.Size.SetPixels(el.Width, el.Height);
        slider.MinValue = el.MinValue;
        slider.MaxValue = el.MaxValue;
        slider.ValueSafe = el.Value;
        if (el.StepsCount > 0) slider.StepsCount = (uint)el.StepsCount;
        slider.MouseWheelStep = el.MouseWheelStep;
        slider.FlippedDirection = el.FlippedDirection;
        return slider;
    }

    private static Picture BuildPicture(UISystem ui, PictureElement el)
    {
        var picture = new Picture(ui);
        picture.Size.SetPixels(el.Width, el.Height);
        return picture;
    }

    private static SlotGrid BuildSlotGrid(UISystem ui, SlotGridElement el)
    {
        var grid = new SlotGrid(ui, el.Columns, el.Rows, el.SlotSize, el.Spacing) { OverrideStyles =
        {
            FillTextureStretched = new StretchedTexture
            {
                TextureId = DefaultSlotGridTexture,
                SourceRect = new Rectangle { Width = 1, Height = 1 }
            }
        } };
        return grid;
    }

    private static ListBox BuildListBox(UISystem ui, ListBoxElement el)
    {
        var list = new ListBox(ui);
        list.Size.SetPixels(el.Width, el.Height);
        list.AllowDeselect = el.AllowDeselect;
        foreach (var item in el.Items) list.AddItem(item);
        if (el.SelectedIndex >= 0 && el.SelectedIndex < el.Items.Count)
            list.SelectedIndex = el.SelectedIndex;
        return list;
    }

    private static DropDown BuildDropDown(UISystem ui, DropDownElement el)
    {
        var drop = new DropDown(ui);
        drop.Size.SetPixels(el.Width, el.Height);
        drop.AllowDeselect = el.AllowDeselect;
        foreach (var item in el.Items) drop.AddItem(item);
        if (el.SelectedIndex >= 0 && el.SelectedIndex < el.Items.Count)
            drop.SelectedIndex = el.SelectedIndex;
        return drop;
    }
}
