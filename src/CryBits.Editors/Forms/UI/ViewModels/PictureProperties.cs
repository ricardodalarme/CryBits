using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class PictureProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    public override string GetTypeDiscriminator() => "Picture";
}
