using CryBits.Client.Framework.Graphics;
using CryBits.Entities;
using SFML.System;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class ItemRenderer(Renderer renderer)
{
    public static ItemRenderer Instance { get; } = new(Renderer.Instance);

    /// <summary>
    /// Render an item icon and its amount at an already-computed screen position.
    /// </summary>
    public void DrawItem(Item item, short amount, Vector2i position)
    {
        if (item == null) return;
        renderer.Draw(Textures.Items[item.Texture], position);
        if (amount > 1) renderer.DrawText(amount.ToString(), position.X + 2, position.Y + 17, Color.White);
    }
}
