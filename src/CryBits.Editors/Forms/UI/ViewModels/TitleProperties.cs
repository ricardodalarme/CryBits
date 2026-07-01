using CryBits.Client.Framework.Persistence.Dtos;
using System.ComponentModel;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class TitleProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly TitleElement _title = (TitleElement)config;

    public override string GetTypeDiscriminator() => "Title";

    [Category("Text")]
    public string Text
    {
        get => _title.Text;
        set
        {
            _title.Text = value;
            if (_entity is Paragraph p)
                p.Text = value;
            RaisePropertyChanged(nameof(Text));
        }
    }
}
