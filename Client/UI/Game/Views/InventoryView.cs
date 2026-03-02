using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Client.Utils.UIUtils;

namespace CryBits.Client.UI.Game.Views;

internal class InventoryView(PlayerSender playerSender, ShopSender shopSender) : IView
{
    internal static Panel Panel => Tools.Panels["Menu_Inventory"];

    public static short CurrentSlot => GetSlotAtMousePosition(Panel, 7, 29, 6, 5);

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

        if (slot == -1) return;
        if (Player.Me.Inventory[slot].Item == null) return;

        switch (e.Button)
        {
            case Mouse.Button.Right:
                if (Player.Me.Inventory[slot].Item.Bind != BindOn.Pickup)
                    // Sell the item if shop is open
                    if (ShopView.Panel.Visible)
                    {
                        if (Player.Me.Inventory[slot].Amount != 1)
                        {
                            ShopSellView.InventorySlot = slot;
                            ShopSellView.AmountTextBox.Text = string.Empty;
                            ShopSellView.Panel.Visible = true;
                        }
                        else shopSender.ShopSell(slot, 1);
                    }
                    // Otherwise drop the item
                    else if (!TradeView.Panel.Visible)
                        if (Player.Me.Inventory[slot].Amount != 1)
                        {
                            DropItemView.InventorySlot = slot;
                            DropItemView.AmountTextBox.Text = string.Empty;
                            DropItemView.Panel.Visible = true;
                        }
                        else playerSender.DropItem(slot, 1);

                break;
            // Select the item (start drag)
            case Mouse.Button.Left:
                GameScreen.InventoryChange = slot;
                break;
        }
    }

    private void OnPanelMouseUp()
    {
        // Return early when no valid slot or no change pending.
        if (CurrentSlot == -1) return;
        if (GameScreen.InventoryChange == -1) return;

        // Send inventory slot change to server.
        playerSender.InventoryChange(GameScreen.InventoryChange, CurrentSlot);
    }

    private void OnPanelMouseDoubleClick(MouseButtonEventArgs e)
    {
        var slot = CurrentSlot;
        if (slot <= 0) return;
        if (Player.Me.Inventory[slot].Item == null) return;

        // Use item
        playerSender.InventoryUse((byte)slot);
    }
}
