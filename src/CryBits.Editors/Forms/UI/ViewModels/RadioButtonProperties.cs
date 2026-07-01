using CryBits.Client.Framework.Persistence.Dtos;
using System.ComponentModel;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class RadioButtonProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly RadioButtonElement _radio = (RadioButtonElement)config;

    public override string GetTypeDiscriminator() => "RadioButton";

    [Category("Text")]
    public string Text
    {
        get => _radio.Text;
        set
        {
            _radio.Text = value;
            if (_entity is RadioButton rb)
                rb.Paragraph.Text = value;
            RaisePropertyChanged(nameof(Text));
        }
    }

    [Category("Checkable")]
    public bool Checked
    {
        get => _radio.Checked;
        set
        {
            _radio.Checked = value;
            if (_entity is CheckedEntity checkable)
                checkable.Checked = value;
            RaisePropertyChanged(nameof(Checked));
        }
    }
}
