using CryBits.Client.Framework.Persistence.Dtos;
using System.ComponentModel;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class PanelProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly PanelElement _panel = (PanelElement)config;

    public override string GetTypeDiscriminator() => "Panel";

    [Category("Texture")]
    public string Texture
    {
        get => _panel.Texture;
        set
        {
            _panel.Texture = value;
            if (_entity.OverrideStyles != null)
                _entity.OverrideStyles.FillTextureStretched = string.IsNullOrEmpty(value) ? null : new global::Iguina.Defs.StretchedTexture
                {
                    TextureId = value,
                    SourceRect = new global::Iguina.Defs.Rectangle { Width = _entity.Size.X.GetValueInPixels(800), Height = _entity.Size.Y.GetValueInPixels(608) }
                };
            RaisePropertyChanged(nameof(Texture));
        }
    }
}
