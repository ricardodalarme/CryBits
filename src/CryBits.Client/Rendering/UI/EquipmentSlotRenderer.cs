using CryBits.Client.Core;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using System.Drawing;

namespace CryBits.Client.Rendering.UI;

internal sealed class EquipmentSlotRenderer(SpriteBatch spriteBatch, GameContext context, DefinitionCatalog catalog)
{
    public void DrawSlot(int slot, Point pos)
    {
        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        var itemId = equipment.Slots[slot];
        if (itemId == Guid.Empty)
            spriteBatch.Draw(Textures.Equipments, pos.X, pos.Y, slot * 32, 0, 32, 32);
        else
        {
            var item = catalog.Items.Get(itemId);
            if (item != null)
                spriteBatch.Draw(Textures.Items[item.Texture], pos);
        }
    }
}
