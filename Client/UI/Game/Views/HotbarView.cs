using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using SFML.Window;

namespace CryBits.Client.UI.Game.Views;

internal class HotbarView(PlayerSender playerSender, ItemRenderer itemRenderer) : IView
{
    internal static Panel Panel => Tools.Panels["Hotbar"];
    private static SlotGrid Grid => Tools.SlotGrids["Hotbar_Grid"];

    public void Bind()
    {
        Grid.OnRenderSlot += OnRenderSlot;
        Grid.OnMouseDown += OnGridMouseDown;
        Grid.OnMouseUp += OnGridMouseUp;
        Grid.OnMouseDoubleClick += OnGridMouseDoubleClick;
    }

    public void Unbind()
    {
        Grid.OnRenderSlot -= OnRenderSlot;
        Grid.OnMouseDown -= OnGridMouseDown;
        Grid.OnMouseUp -= OnGridMouseUp;
        Grid.OnMouseDoubleClick -= OnGridMouseDoubleClick;
    }

    private void OnRenderSlot(int slot, Point pos)
    {
        var hotbarSlot = Player.Me.Hotbar[slot];
        if (hotbarSlot.Slot > 0 && hotbarSlot.Type == SlotType.Item)
            itemRenderer.DrawItem(Player.Me.Inventory[hotbarSlot.Slot].Item, 1, pos);
    }

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        if (Player.Me.Hotbar[slot].Slot == 0) return;

        switch (e.Button)
        {
            // Drop or use hotbar slot (right-click)
            case Mouse.Button.Right:
                playerSender.HotbarAdd(slot, 0, 0);
                break;
            // Select the hotbar slot (start drag)
            case Mouse.Button.Left:
                GameScreen.HotbarChange = slot;
                break;
        }
    }

    private void OnGridMouseUp(short slot)
    {
        if (GameScreen.HotbarChange >= 0) playerSender.HotbarChange(GameScreen.HotbarChange, slot);
        if (GameScreen.InventoryChange > 0) playerSender.HotbarAdd(slot, (byte)SlotType.Item, GameScreen.InventoryChange);
    }

    private void OnGridMouseDoubleClick(MouseButtonEventArgs e, short slot)
    {
        if (Player.Me.Hotbar[slot].Slot <= 0) return;

        // Use item from hotbar
        playerSender.HotbarUse((byte)slot);
    }
}
