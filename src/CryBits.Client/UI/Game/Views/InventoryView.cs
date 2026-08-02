using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Items;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class InventoryView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    TooltipView tooltip,
    ShopView shop,
    GameScreen gameScreen,
    InventoryViewModel viewModel) : ViewBase
{
    internal InventoryViewModel ViewModel => viewModel;

    private SlotGrid Grid => uiContext.Get<SlotGrid>("InventoryGrid");

    internal short? DragOrigin { get; private set; }

    public void ClearDrag() => DragOrigin = null;

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
        var item = viewModel.Slots[slot];
        if (item == null || item.ItemId == Guid.Empty) return;

        DragOrigin = (short)slot;
        gameScreen.InventoryChange = (short)slot;
    }

    private void OnSlotLeftUp(int slot)
    {
        var dragSlot = DragOrigin;
        DragOrigin = null;
        gameScreen.InventoryChange = null;
        if (dragSlot == null) return;

        viewModel.Swap(dragSlot.Value, (short)slot);
        uiContext.Registry["Drop"].Visible = false;
    }

    private void OnSlotRightClick(int slot)
    {
        var itemVM = viewModel.Slots[slot];
        if (itemVM == null || itemVM.ItemId == Guid.Empty) return;

        var item = itemVM.Definition;
        if (item?.Bind == BindOn.Pickup) return;

        if (uiContext.Registry["Shop"].Visible)
        {
            if (itemVM.Amount != 1)
            {
                gameScreen.ShopSellView.Show((short)slot);
                gameScreen.ShopSellView.AmountInput.Value = string.Empty;
            }
            else
            {
                viewModel.Sell((short)slot, 1);
            }
        }
        else if (!uiContext.Registry["Trade"].Visible)
        {
            if (itemVM.Amount != 1)
            {
                gameScreen.DropItemView.Show((short)slot);
                gameScreen.DropItemView.AmountInput.Value = string.Empty;
            }
            else
            {
                viewModel.Drop((short)slot, 1);
            }
        }
    }

    private void OnSlotDoubleClick(int slot)
    {
        viewModel.Use((short)slot);
        uiContext.Registry["Drop"].Visible = false;
    }

    private void OnSlotHoverEnter(int slot)
    {
        var itemVM = viewModel.Slots[slot];
        if (itemVM == null || itemVM.ItemId == Guid.Empty) return;
        var item = itemVM.Definition;
        if (item == null) return;

        string? additionalInfo = null;
        if (shop.TryGetSalePrice(item.Id, out var price))
            additionalInfo = "Sale price: " + price;

        var panelRect = uiContext.Registry["InventoryPanel"].LastBoundingRect;
        tooltip.Show(item.Id, new Point(panelRect.X - 186, panelRect.Y + 3), additionalInfo);
    }

    private void OnPostDraw()
    {
        if (!uiContext.Registry["InventoryPanel"].Visible) return;

        for (var i = 0; i < Grid.TotalSlots; i++)
        {
            var rect = Grid.GetSlotRect(i);
            var itemVM = viewModel.Slots[i];
            if (itemVM == null || itemVM.ItemId == Guid.Empty) continue;
            var item = itemVM.Definition;
            if (item != null)
                itemRenderer.DrawItem(item, itemVM.Amount, new Point(rect.X, rect.Y));
        }
    }
}
