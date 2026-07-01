using CryBits.Client.Framework.Persistence.Dtos;
using System.ComponentModel;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class ButtonProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly ButtonElement _button = (ButtonElement)config;

    public override string GetTypeDiscriminator() => "Button";

    [Category("Texture")]
    public string Texture
    {
        get => _button.Texture;
        set
        {
            _button.Texture = value;
            RaisePropertyChanged(nameof(Texture));
        }
    }

    [Category("Checkable")]
    public bool Checked
    {
        get => _button.Checked;
        set
        {
            _button.Checked = value;
            if (_entity is CheckedEntity checkable)
                checkable.Checked = value;
            RaisePropertyChanged(nameof(Checked));
        }
    }

    [Category("Checkable")]
    [DisplayName("Toggle On Click")]
    public bool ToggleCheckOnClick
    {
        get => _button.ToggleCheckOnClick;
        set
        {
            _button.ToggleCheckOnClick = value;
            if (_entity is CheckedEntity checkable)
                checkable.ToggleCheckOnClick = value;
            RaisePropertyChanged(nameof(ToggleCheckOnClick));
        }
    }

    [Category("Checkable")]
    [DisplayName("Exclusive Selection")]
    public bool ExclusiveSelection
    {
        get => _button.ExclusiveSelection;
        set
        {
            _button.ExclusiveSelection = value;
            if (_entity is CheckedEntity checkable)
                checkable.ExclusiveSelection = value;
            RaisePropertyChanged(nameof(ExclusiveSelection));
        }
    }

    [Category("Checkable")]
    [DisplayName("Can Click To Uncheck")]
    public bool CanClickToUncheck
    {
        get => _button.CanClickToUncheck;
        set
        {
            _button.CanClickToUncheck = value;
            if (_entity is CheckedEntity checkable)
                checkable.CanClickToUncheck = value;
            RaisePropertyChanged(nameof(CanClickToUncheck));
        }
    }
}
