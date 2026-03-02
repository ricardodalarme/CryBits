using System.Drawing;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Managers;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Utils;

internal static class UIUtils
{
    public static short GetSlotAtMousePosition(Panel panel, byte offX, byte offY, byte lines, byte columns, byte grid = 32, byte gap = 4)
    {
        var size = grid + gap;
        var start = panel.Position + new Size(offX, offY);
        var slot = new Point((InputManager.Instance.MousePosition.X - start.X) / size,
            (InputManager.Instance.MousePosition.Y - start.Y) / size);

        // Check whether the mouse is over the slot
        if (slot.Y < 0 || slot.X < 0 || slot.X >= columns || slot.Y >= lines) return -1;
        if (!IsAbove(new Rectangle(start.X + slot.X * size, start.Y + slot.Y * size, grid, grid))) return -1;
        if (!panel.Visible) return -1;

        // Return slot index
        return (short)(slot.Y * columns + slot.X);
    }
}