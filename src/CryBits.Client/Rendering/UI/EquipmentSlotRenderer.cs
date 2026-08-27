using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Client.Rendering.UI;

internal sealed class EquipmentSlotRenderer(SpriteBatch spriteBatch)
{
    public void DrawSlot(int slot, Item? item, Vector2 pos)
    {
        if (item == null)
        {
            spriteBatch.Draw(Textures.Equipments,
                new Rectangle((int)pos.X, (int)pos.Y, 32, 32),
                new Rectangle(slot * 32, 0, 32, 32),
                Color.White);
        }
        else
        {
            spriteBatch.Draw(Textures.Items[item.Texture], pos, Color.White);
        }
    }
}
