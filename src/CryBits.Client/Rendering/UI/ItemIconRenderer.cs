using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Items;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Client.Rendering.UI;

internal sealed class ItemIconRenderer(SpriteBatch spriteBatch)
{
    public Texture2D? GetTexture(Item item) => item == null ? null : Textures.Items[item.Texture];

    /// <summary>
    /// Render an item icon and its amount at an already-computed screen position.
    /// </summary>
    public void DrawItem(Item item, short amount, Vector2 position)
    {
        if (item == null) return;
        spriteBatch.Draw(Textures.Items[item.Texture], position, Color.White);
        if (amount > 1)
            spriteBatch.DrawString(Fonts.Default, amount.ToString(), new Vector2(position.X + 2, position.Y + 17), Color.White);
    }
}
