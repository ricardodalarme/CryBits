using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Items;
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Rendering.UI;

internal sealed class ItemIconRenderer(SpriteBatch spriteBatch)
{
    /// <summary>
    /// Render an item icon and its amount at an already-computed screen position.
    /// </summary>
    public void DrawItem(Item item, short amount, Point position)
    {
        if (item == null) return;
        spriteBatch.Draw(Textures.Items[item.Texture], position);
        if (amount > 1) spriteBatch.DrawText(amount.ToString(), position.X + 2, position.Y + 17, Color.White);
    }
}
