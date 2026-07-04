using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class LabelProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly LabelElement _label = (LabelElement)config;

    public override string GetTypeDiscriminator() => "Label";

    [Category("Text")]
    public string Text
    {
        get => _label.Text;
        set
        {
            _label.Text = value;
            if (_entity is Paragraph p)
                p.Text = value;
            RaisePropertyChanged(nameof(Text));
        }
    }

    [Category("Text")]
    [DisplayName("Max Width")]
    [Range(0, 9999)]
    public int MaxWidth
    {
        get => _label.MaxWidth;
        set
        {
            _label.MaxWidth = value;
            RaisePropertyChanged(nameof(MaxWidth));
        }
    }
}
