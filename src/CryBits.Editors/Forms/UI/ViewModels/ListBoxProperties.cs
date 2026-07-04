using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal class ListBoxProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly ListBoxElement _list = (ListBoxElement)config;

    public override string GetTypeDiscriminator() => "ListBox";

    [Category("List")]
    [DisplayName("Selected Index")]
    public int SelectedIndex
    {
        get => _list.SelectedIndex;
        set
        {
            _list.SelectedIndex = value;
            if (_entity is ListBox lb)
                lb.SelectedIndex = value;
            RaisePropertyChanged(nameof(SelectedIndex));
        }
    }

    [Category("List")]
    [DisplayName("Allow Deselect")]
    public bool AllowDeselect
    {
        get => _list.AllowDeselect;
        set
        {
            _list.AllowDeselect = value;
            if (_entity is ListBox lb)
                lb.AllowDeselect = value;
            RaisePropertyChanged(nameof(AllowDeselect));
        }
    }
}
