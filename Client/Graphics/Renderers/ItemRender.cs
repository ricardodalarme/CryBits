using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Entities;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal static class ItemRenderer
{
  public static void Item(Item item, short amount, Point start, byte slot, byte columns, byte grid = 32,
    byte gap = 4)
  {
    // Somente se necessário
    if (item == null) return;

    // Posição do item baseado no slot
    var line = (slot - 1) / columns;
    var column = slot - line * 5 - 1;
    var position = start + new Size(column * (grid + gap), line * (grid + gap));

    // Desenha o item e sua quantidade
    Renders.Render(Textures.Items[item.Texture], position);
    if (amount > 1) Renders.DrawText(amount.ToString(), position.X + 2, position.Y + 17, Color.White);
  }
}