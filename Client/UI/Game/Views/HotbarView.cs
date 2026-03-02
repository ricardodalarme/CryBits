using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Client.Utils.UIUtils;

namespace CryBits.Client.UI.Game.Views;

internal class HotbarView(PlayerSender playerSender) : IView
{
    internal static Panel Panel => Tools.Panels["Hotbar"];

    public static short CurrentSlot => GetSlotAtMousePosition(Panel, 8, 6, 1, 10);

    public void Bind()
    {
        Panel.OnMouseDown += OnPanelMouseDown;
        Panel.OnMouseUp += OnPanelMouseUp;
        Panel.OnMouseDoubleClick += OnPanelMouseDoubleClick;
    }

    public void Unbind()
    {
        Panel.OnMouseDown -= OnPanelMouseDown;
        Panel.OnMouseUp -= OnPanelMouseUp;
        Panel.OnMouseDoubleClick -= OnPanelMouseDoubleClick;
    }

    private void OnPanelMouseDown(MouseButtonEventArgs e)
    {
        var slot = CurrentSlot;

        if (slot < 0) return;
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

    private void OnPanelMouseUp()
    {
        // Change hotbar slot
        if (CurrentSlot < 0) return;
        if (GameScreen.HotbarChange >= 0) playerSender.HotbarChange(GameScreen.HotbarChange, CurrentSlot);
        if (GameScreen.InventoryChange > 0) playerSender.HotbarAdd(CurrentSlot, (byte)SlotType.Item, GameScreen.InventoryChange);
    }

    private void OnPanelMouseDoubleClick(MouseButtonEventArgs e)
    {
        var slot = CurrentSlot;
        if (slot <= 0) return;
        if (Player.Me.Hotbar[slot].Slot <= 0) return;

        // Use item from hotbar
        playerSender.HotbarUse((byte)slot);
    }
}
