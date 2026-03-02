using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Enums;

namespace CryBits.Client.UI.Game.Views;

internal class DraggableSlotView(ItemRenderer itemRenderer) : IView
{
    private static Picture DraggableSlotPicture => Tools.Pictures["DraggableSlot"];

    public void Bind() => DraggableSlotPicture.OnRender += OnRender;

    public void Unbind() => DraggableSlotPicture.OnRender -= OnRender;

    private void OnRender(Point _)
    {
        var pos = new Point(
            InputManager.Instance.MousePosition.X + 6,
            InputManager.Instance.MousePosition.Y + 6
        );

        if (GameScreen.HotbarChange >= 0)
        {
            if (Player.Me.Hotbar[GameScreen.HotbarChange].Type == SlotType.Item)
                itemRenderer.DrawItem(Player.Me.Inventory[Player.Me.Hotbar[GameScreen.HotbarChange].Slot].Item, 1, pos);
        }
        else if (GameScreen.InventoryChange > 0)
        {
            itemRenderer.DrawItem(Player.Me.Inventory[GameScreen.InventoryChange].Item, 1, pos);
        }
    }
}
