using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Items;
using System.Drawing;

namespace CryBits.Client.Rendering.UI;

internal sealed class EquipmentSlotRenderer(SpriteBatch spriteBatch)
{
    public void DrawSlot(int slot, Item? item, Point pos)
    {
        if (item == null)
        {
            spriteBatch.Draw(Textures.Equipments, pos.X, pos.Y, slot * 32, 0, 32, 32);
        }
        else
        {
            spriteBatch.Draw(Textures.Items[item.Texture], pos);
        }
    }
}
