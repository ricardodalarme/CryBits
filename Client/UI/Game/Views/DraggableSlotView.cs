using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using CryBits.Enums;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class DraggableSlotView(ItemRenderer itemRenderer, InputManager inputManager, GameContext context) : IView
{
    private static Picture DraggableSlotPicture => Tools.Pictures["DraggableSlot"];

    public void Bind() => DraggableSlotPicture.OnRender += OnRender;

    public void Unbind() => DraggableSlotPicture.OnRender -= OnRender;

    private void OnRender(Point _)
    {
        var pos = new Point(
            inputManager.MousePosition.X + 6,
            inputManager.MousePosition.Y + 6
        );

        if (GameScreen.HotbarChange != null)
        {
            var hotbarSlot = context.LocalPlayer.GetHotbar().Slots[GameScreen.HotbarChange.Value];
            if (hotbarSlot?.Type == SlotType.Item)
                itemRenderer.DrawItem(context.LocalPlayer.GetInventory().Slots[hotbarSlot.Slot]?.Item, 1, pos);
        }
        else if (GameScreen.InventoryChange != null)
        {
            itemRenderer.DrawItem(context.LocalPlayer.GetInventory().Slots[GameScreen.InventoryChange.Value]?.Item, 1, pos);
        }
    }
}
