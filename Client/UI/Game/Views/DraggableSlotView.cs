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

        if (GameScreen.HotbarChange >= 0)
        {
            var hotbarSlot = context.LocalPlayer.GetHotbar().Slots[GameScreen.HotbarChange];
            if (hotbarSlot?.Type == SlotType.Item)
                itemRenderer.DrawItem(context.LocalPlayer.GetInventory().Slots[hotbarSlot.Slot]?.Item, 1, pos);
        }
        else if (GameScreen.InventoryChange > 0)
        {
            itemRenderer.DrawItem(context.LocalPlayer.GetInventory().Slots[GameScreen.InventoryChange]?.Item, 1, pos);
        }
    }
}
