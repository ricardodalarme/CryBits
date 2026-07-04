using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class CheckboxProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly CheckboxElement _checkbox = (CheckboxElement)config;

    public override string GetTypeDiscriminator() => "Checkbox";

    [Category("Text")]
    public string Text
    {
        get => _checkbox.Text;
        set
        {
            _checkbox.Text = value;
            if (_entity is Checkbox cb)
                cb.Paragraph.Text = value;
            RaisePropertyChanged(nameof(Text));
        }
    }

    [Category("Checkable")]
    public bool Checked
    {
        get => _checkbox.Checked;
        set
        {
            _checkbox.Checked = value;
            if (_entity is CheckedEntity checkable)
                checkable.Checked = value;
            RaisePropertyChanged(nameof(Checked));
        }
    }

    [Category("Checkable")]
    [DisplayName("Exclusive Selection")]
    public bool ExclusiveSelection
    {
        get => _checkbox.ExclusiveSelection;
        set
        {
            _checkbox.ExclusiveSelection = value;
            if (_entity is CheckedEntity checkable)
                checkable.ExclusiveSelection = value;
            RaisePropertyChanged(nameof(ExclusiveSelection));
        }
    }
}
