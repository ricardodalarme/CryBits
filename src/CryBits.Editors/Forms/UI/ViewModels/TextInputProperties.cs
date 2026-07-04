using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal class TextInputProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly TextInputElement _input = (TextInputElement)config;

    public override string GetTypeDiscriminator() => "TextInput";

    [Category("Text Input")]
    [DisplayName("Placeholder")]
    public string PlaceholderText
    {
        get => _input.PlaceholderText;
        set
        {
            _input.PlaceholderText = value;
            if (_entity is TextInput ti)
                ti.PlaceholderText = value;
            RaisePropertyChanged(nameof(PlaceholderText));
        }
    }

    [Category("Text Input")]
    [DisplayName("Max Length")]
    [Range(0, 9999)]
    public int MaxLength
    {
        get => _input.MaxLength;
        set
        {
            _input.MaxLength = value;
            if (_entity is TextInput ti)
                ti.MaxLength = value <= 0 ? null : value;
            RaisePropertyChanged(nameof(MaxLength));
        }
    }

    [Category("Text Input")]
    public bool Masked
    {
        get => _input.Masked;
        set
        {
            _input.Masked = value;
            if (_entity is TextInput ti)
                ti.MaskingCharacter = value ? '*' : null;
            RaisePropertyChanged(nameof(Masked));
        }
    }
}
