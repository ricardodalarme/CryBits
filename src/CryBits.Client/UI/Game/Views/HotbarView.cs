using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class HotbarView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    TooltipView tooltip,
    InventoryView inventory,
    GameScreen gameScreen,
    HotbarViewModel viewModel) : ViewBase
{
    private SlotGrid Grid => uiContext.Get<SlotGrid>("HotbarGrid");

    private short? _hotbarDragOrigin;

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
        viewModel.Refresh();
        var hbSlot = viewModel.Slots[slot];
        if (hbSlot == null || hbSlot.Slot <= 0) return;

        _hotbarDragOrigin = (short)slot;
        gameScreen.HotbarChange = (short)slot;
    }

    private void OnSlotLeftUp(int slot)
    {
        var hotSlot = _hotbarDragOrigin;
        _hotbarDragOrigin = null;
        gameScreen.HotbarChange = null;
        gameScreen.InventoryChange = null;
        if (hotSlot is not null)
            viewModel.Swap(hotSlot.Value, (short)slot);

        var invSlot = inventory.DragOrigin;
        if (invSlot is not null)
            viewModel.AddItem((short)slot, invSlot.Value);
    }

    private void OnSlotRightClick(int slot)
    {
        viewModel.Remove((short)slot);
    }

    private void OnSlotDoubleClick(int slot)
    {
        viewModel.Refresh();
        var hbSlot = viewModel.Slots[slot];
        if (hbSlot is { Slot: > 0 })
        {
            viewModel.Use((short)slot);
            uiContext.Registry["Drop"].Visible = false;
        }
    }

    private void OnSlotHoverEnter(int slot)
    {
        viewModel.Refresh();
        var hbSlot = viewModel.Slots[slot];
        if (hbSlot is { Slot: > 0, Type: SlotType.Item } h)
        {
            var item = h.Definition;
            if (item == null) return;
            var panelRect = uiContext.Registry["HotbarPanel"].LastBoundingRect;
            tooltip.Show(item.Id, new Point(panelRect.X, panelRect.Y + 42));
        }
    }

    private void OnPostDraw()
    {
        if (!uiContext.Registry["HotbarPanel"].Visible) return;

        viewModel.Refresh();

        for (var i = 0; i < Grid.TotalSlots; i++)
        {
            var rect = Grid.GetSlotRect(i);
            var hbSlot = viewModel.Slots[i];
            if (hbSlot is { Slot: > 0, Type: SlotType.Item } && hbSlot.Definition is { } item)
            {
                itemRenderer.DrawItem(item, 1, new Point(rect.X, rect.Y));
            }
        }
    }
}
