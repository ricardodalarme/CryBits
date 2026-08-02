using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using System.Drawing;

namespace CryBits.Client.Rendering.UI;

internal sealed class EquipmentSlotRenderer(SpriteBatch spriteBatch, DefinitionCatalog catalog)
{
    public void DrawSlot(int slot, Guid itemId, Point pos)
    {
        if (itemId == Guid.Empty)
        {
            spriteBatch.Draw(Textures.Equipments, pos.X, pos.Y, slot * 32, 0, 32, 32);
        }
        else
        {
            var item = catalog.Items.Get(itemId);
            if (item != null)
                spriteBatch.Draw(Textures.Items[item.Texture], pos);
        }
    }
}
