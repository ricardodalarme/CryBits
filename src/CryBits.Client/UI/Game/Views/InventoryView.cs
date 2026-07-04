using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Intents;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class InventoryView(UiContext uiContext, IntentSender intentSender, ItemIconRenderer itemRenderer, GameContext context, DefinitionCatalog catalog, TooltipView tooltip, ShopView shop, GameScreen gameScreen) : ViewBase
{
    private SlotGrid Grid => uiContext.Get<SlotGrid>("InventoryGrid");

    private short? _dragOrigin;
    internal short? DragOrigin => _dragOrigin;
    public void ClearDrag() => _dragOrigin = null;

    public override void Bind()
    {
        Grid.OnSlotLeftDown += OnSlotLeftDown;
        Grid.OnSlotLeftUp += OnSlotLeftUp;
        Grid.OnSlotRightClick += OnSlotRightClick;
        Grid.OnSlotDoubleClick += OnSlotDoubleClick;
        Grid.OnSlotHoverEnter += OnSlotHoverEnter;
        Grid.OnSlotHoverLeave += tooltip.Hide;
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        Grid.OnSlotLeftDown -= OnSlotLeftDown;
        Grid.OnSlotLeftUp -= OnSlotLeftUp;
        Grid.OnSlotRightClick -= OnSlotRightClick;
        Grid.OnSlotDoubleClick -= OnSlotDoubleClick;
        Grid.OnSlotHoverEnter -= OnSlotHoverEnter;
        Grid.OnSlotHoverLeave -= tooltip.Hide;
        uiContext.PostDraw -= OnPostDraw;
    }

    private void OnSlotLeftDown(int slot)
    {
        var inv = context.LocalPlayer.GetInventory();
        if (inv == null || inv.Slots[slot].ItemId == Guid.Empty) return;

        _dragOrigin = (short)slot;
        gameScreen.InventoryChange = (short)slot;
    }

    private void OnSlotLeftUp(int slot)
    {
        var dragSlot = _dragOrigin;
        _dragOrigin = null;
        gameScreen.InventoryChange = null;
        if (dragSlot == null) return;

        intentSender.Send(new InventorySwapIntent(default, dragSlot.Value, (short)slot));
        uiContext.Registry["Drop"].Visible = false;
    }

    private void OnSlotRightClick(int slot)
    {
        var inv = context.LocalPlayer.GetInventory();
        if (inv == null || inv.Slots[slot].ItemId == Guid.Empty) return;

        var item = catalog.Items.Get(inv.Slots[slot].ItemId);
        if (item?.Bind == BindOn.Pickup) return;

        if (uiContext.Registry["Shop"].Visible)
        {
            if (inv.Slots[slot].Amount != 1)
            {
                gameScreen.ShopSellView.Show((short)slot);
                gameScreen.ShopSellView.AmountInput.Value = string.Empty;
            }
            else intentSender.Send(new ShopSellIntent(default, (byte)slot, 1));
        }
        else if (!uiContext.Registry["Trade"].Visible)
        {
            if (inv.Slots[slot].Amount != 1)
            {
                gameScreen.DropItemView.Show((short)slot);
                gameScreen.DropItemView.AmountInput.Value = string.Empty;
            }
            else intentSender.Send(new DropItemIntent(default, (byte)slot, 1));
        }
    }

    private void OnSlotDoubleClick(int slot)
    {
        intentSender.Send(new InventoryUseIntent(default, (byte)slot));
        uiContext.Registry["Drop"].Visible = false;
    }

    private void OnSlotHoverEnter(int slot)
    {
        var inv = context.LocalPlayer.GetInventory();
        if (inv == null) return;
        var item = catalog.Items.Get(inv.Slots[slot].ItemId);
        if (item == null) return;

        string? additionalInfo = null;
        if (uiContext.Registry["Shop"].Visible &&
            shop.OpenedShop?.FindBought(item.Id) != null)
            additionalInfo = "Sale price: " + shop.OpenedShop.FindBought(item.Id).Price;

        var panelRect = uiContext.Registry["InventoryPanel"].LastBoundingRect;
        tooltip.Show(item.Id, new Point(panelRect.X - 186, panelRect.Y + 3), additionalInfo);
    }

    private void OnPostDraw()
    {
        if (!uiContext.Registry["InventoryPanel"].Visible) return;

        var inv = context.LocalPlayer.GetInventory();
        if (inv == null) return;

        for (var i = 0; i < Grid.TotalSlots; i++)
        {
            var rect = Grid.GetSlotRect(i);
            var s = inv.Slots[i];
            if (s.ItemId == Guid.Empty) continue;
            if (catalog.Items.Get(s.ItemId) is { } item)
                itemRenderer.DrawItem(item, s.Amount, new Point(rect.X, rect.Y));
        }
    }
}
