using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class EquipmentRenderer(Renderer renderer)
{
    public static EquipmentRenderer Instance { get; } = new(Renderer.Instance);

    public void DrawSlot(int slot, Point pos)
    {
        if (Player.Me.Equipment[slot] == null)
            renderer.Draw(Textures.Equipments, pos.X, pos.Y, slot * 32, 0, 32, 32);
        else
            renderer.Draw(Textures.Items[Player.Me.Equipment[slot].Texture], pos);
    }
}
