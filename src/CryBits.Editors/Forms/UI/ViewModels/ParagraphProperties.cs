using CryBits.Client.Framework.Persistence.Dtos;
using System.ComponentModel;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class ParagraphProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly ParagraphElement _paragraph = (ParagraphElement)config;

    public override string GetTypeDiscriminator() => "Paragraph";

    [Category("Text")]
    public string Text
    {
        get => _paragraph.Text;
        set
        {
            _paragraph.Text = value;
            if (_entity is Paragraph p)
                p.Text = value;
            RaisePropertyChanged(nameof(Text));
        }
    }

    [Category("Text")]
    [DisplayName("Enable Style Commands")]
    public bool EnableStyleCommands
    {
        get => _paragraph.EnableStyleCommands;
        set
        {
            _paragraph.EnableStyleCommands = value;
            if (_entity is Paragraph p)
                p.EnableStyleCommands = value;
            RaisePropertyChanged(nameof(EnableStyleCommands));
        }
    }
}
