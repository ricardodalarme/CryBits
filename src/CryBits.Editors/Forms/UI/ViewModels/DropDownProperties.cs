using CryBits.Client.Framework.Persistence.Dtos;
using System.ComponentModel;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class DropDownProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly DropDownElement _dropdown = (DropDownElement)config;

    public override string GetTypeDiscriminator() => "DropDown";

    [Category("List")]
    [DisplayName("Selected Index")]
    public int SelectedIndex
    {
        get => _dropdown.SelectedIndex;
        set
        {
            _dropdown.SelectedIndex = value;
            if (_entity is ListBox lb)
                lb.SelectedIndex = value;
            RaisePropertyChanged(nameof(SelectedIndex));
        }
    }

    [Category("List")]
    [DisplayName("Allow Deselect")]
    public bool AllowDeselect
    {
        get => _dropdown.AllowDeselect;
        set
        {
            _dropdown.AllowDeselect = value;
            if (_entity is ListBox lb)
                lb.AllowDeselect = value;
            RaisePropertyChanged(nameof(AllowDeselect));
        }
    }
}
