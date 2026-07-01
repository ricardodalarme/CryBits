using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal static class ElementViewModelFactory
{
    private static readonly Dictionary<Type, Func<Element, Entity, UIElementProperties>> Creators = new()
    {
        [typeof(PanelElement)] = (c, e) => new PanelProperties(c, e),
        [typeof(ButtonElement)] = (c, e) => new ButtonProperties(c, e),
        [typeof(CheckboxElement)] = (c, e) => new CheckboxProperties(c, e),
        [typeof(RadioButtonElement)] = (c, e) => new RadioButtonProperties(c, e),
        [typeof(LabelElement)] = (c, e) => new LabelProperties(c, e),
        [typeof(TitleElement)] = (c, e) => new TitleProperties(c, e),
        [typeof(ParagraphElement)] = (c, e) => new ParagraphProperties(c, e),
        [typeof(TextInputElement)] = (c, e) => new TextInputProperties(c, e),
        [typeof(NumericInputElement)] = (c, e) => new NumericInputProperties(c, e),
        [typeof(ProgressBarElement)] = (c, e) => new ProgressBarProperties(c, e),
        [typeof(SliderElement)] = (c, e) => new SliderProperties(c, e),
        [typeof(PictureElement)] = (c, e) => new PictureProperties(c, e),
        [typeof(SlotGridElement)] = (c, e) => new SlotGridProperties(c, e),
        [typeof(ListBoxElement)] = (c, e) => new ListBoxProperties(c, e),
        [typeof(DropDownElement)] = (c, e) => new DropDownProperties(c, e),
    };

    private static readonly Dictionary<string, Type> DiscriminatorToDto = new()
    {
        ["Panel"] = typeof(PanelElement),
        ["Button"] = typeof(ButtonElement),
        ["Checkbox"] = typeof(CheckboxElement),
        ["RadioButton"] = typeof(RadioButtonElement),
        ["Label"] = typeof(LabelElement),
        ["Title"] = typeof(TitleElement),
        ["Paragraph"] = typeof(ParagraphElement),
        ["TextInput"] = typeof(TextInputElement),
        ["NumericInput"] = typeof(NumericInputElement),
        ["ProgressBar"] = typeof(ProgressBarElement),
        ["Slider"] = typeof(SliderElement),
        ["Picture"] = typeof(PictureElement),
        ["SlotGrid"] = typeof(SlotGridElement),
        ["ListBox"] = typeof(ListBoxElement),
        ["DropDown"] = typeof(DropDownElement),
    };

    private static readonly Dictionary<Type, string> DtoToDiscriminator = DiscriminatorToDto
        .ToDictionary(kv => kv.Value, kv => kv.Key);

    public static UIElementProperties Create(Element config, Entity entity)
    {
        if (Creators.TryGetValue(config.GetType(), out var factory))
            return factory(config, entity);
        return new PanelProperties(config, entity);
    }

    public static string GetDiscriminator(Element config) =>
        DtoToDiscriminator.TryGetValue(config.GetType(), out var d) ? d : "Panel";

    public static Element CreateDefault(string discriminator)
    {
        if (!DiscriminatorToDto.TryGetValue(discriminator, out var type))
            type = typeof(PanelElement);

        return (Element)(Activator.CreateInstance(type) ?? new PanelElement());
    }
}
