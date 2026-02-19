using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Entities;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal static class ItemRenderer
{
    /// <summary>
    /// Render an item icon and its amount at the specified slot position.
    /// </summary>
    public static void Item(Item item, short amount, Point start, byte slot, byte columns, byte grid = 32,
      byte gap = 4)
    {
        if (item == null) return;

        var line = (slot - 1) / columns;
        var column = slot - line * 5 - 1;
        var position = start + new Size(column * (grid + gap), line * (grid + gap));

        Renders.Render(Textures.Items[item.Texture], position);
        if (amount > 1) Renders.DrawText(amount.ToString(), position.X + 2, position.Y + 17, Color.White);
    }
}
