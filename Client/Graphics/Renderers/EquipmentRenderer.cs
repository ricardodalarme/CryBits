using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Worlds;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class EquipmentRenderer(Renderer renderer, GameContext context)
{
    public static EquipmentRenderer Instance { get; } = new(Renderer.Instance, GameContext.Instance);

    public void DrawSlot(int slot, Point pos)
    {
        var item = context.LocalPlayer.GetEquipment().Slots[slot];
        if (item == null)
            renderer.Draw(Textures.Equipments, pos.X, pos.Y, slot * 32, 0, 32, 32);
        else
            renderer.Draw(Textures.Items[item.Texture], pos);
    }
}
