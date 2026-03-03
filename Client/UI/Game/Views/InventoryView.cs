using System.Drawing;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Enums;
using SFML.Window;

namespace CryBits.Client.UI.Game.Views;

internal class InventoryView(PlayerSender playerSender, ShopSender shopSender, ItemRenderer itemRenderer) : IView
{
    internal static Panel Panel => Tools.Panels["Menu_Inventory"];
    private static SlotGrid Grid => Tools.SlotGrids["Inventory_Grid"];

    public void Bind()
    {
        Grid.OnRenderSlot += OnRenderSlot;
        Grid.OnMouseDown += OnGridMouseDown;
        Grid.OnMouseUp += OnGridMouseUp;
        Grid.OnMouseDoubleClick += OnGridMouseDoubleClick;
        Grid.OnSlotHover += OnGridSlotHover;
        Grid.OnSlotLeave += OnGridSlotLeave;
    }

    public void Unbind()
    {
        Grid.OnRenderSlot -= OnRenderSlot;
        Grid.OnMouseDown -= OnGridMouseDown;
        Grid.OnMouseUp -= OnGridMouseUp;
        Grid.OnMouseDoubleClick -= OnGridMouseDoubleClick;
        Grid.OnSlotHover -= OnGridSlotHover;
        Grid.OnSlotLeave -= OnGridSlotLeave;
    }

    private void OnRenderSlot(int slot, Point pos)
    {
        ref var inv = ref GameContext.Instance.LocalPlayer.GetInventory();
        itemRenderer.DrawItem(inv.Slots[slot]?.Item, inv.Slots[slot]?.Amount ?? 0, pos);
    }

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        ref var inv = ref GameContext.Instance.LocalPlayer.GetInventory();
        if (inv.Slots[slot]?.Item == null) return;

        switch (e.Button)
        {
            case Mouse.Button.Right:
                if (inv.Slots[slot].Item.Bind != BindOn.Pickup)
                    // Sell the item if shop is open
                    if (ShopView.Panel.Visible)
                    {
                        if (inv.Slots[slot].Amount != 1)
                        {
                            ShopSellView.InventorySlot = slot;
                            ShopSellView.AmountTextBox.Text = string.Empty;
                            ShopSellView.Panel.Visible = true;
                        }
                        else shopSender.ShopSell(slot, 1);
                    }
                    // Otherwise drop the item
                    else if (!TradeView.Panel.Visible)
                        if (inv.Slots[slot].Amount != 1)
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

    private void OnGridMouseUp(short slot)
    {
        if (GameScreen.InventoryChange == -1) return;

        // Send inventory slot change to server.
        playerSender.InventoryChange(GameScreen.InventoryChange, slot);
    }

    private void OnGridMouseDoubleClick(MouseButtonEventArgs e, short slot)
    {
        if (slot <= 0) return;
        if (GameContext.Instance.LocalPlayer.GetInventory().Slots[slot]?.Item == null) return;

        // Use item
        playerSender.InventoryUse((byte)slot);
    }

    private static void OnGridSlotHover(short slot)
    {
        var item = GameContext.Instance.LocalPlayer.GetInventory().Slots[slot]?.Item;
        if (item == null) return;
        string? context = null;
        if (ShopView.Panel.Visible && ShopView.OpenedShop?.FindBought(item) != null)
            context = "Sale price: " + ShopView.OpenedShop.FindBought(item).Price;
        InformationView.Show(item.Id, Panel.Position + new Size(-186, 3), context);
    }

    private static void OnGridSlotLeave(short slot) => InformationView.Hide();
}
