using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class HotbarView(UiContext uiContext, IntentSender intentSender, ItemRenderer itemRenderer, GameContext context, DefinitionCatalog catalog, TooltipView tooltip, InventoryView inventory, GameScreen gameScreen) : ViewBase
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
        var hotbarSlot = context.LocalPlayer.GetHotbar()?.Slots[slot];
        if (hotbarSlot is not HotbarSlot { Slot: not 0 }) return;

        _hotbarDragOrigin = (short)slot;
        gameScreen.HotbarChange = (short)slot;
    }

    private void OnSlotLeftUp(int slot)
    {
        var hotSlot = _hotbarDragOrigin;
        _hotbarDragOrigin = null;
        gameScreen.HotbarChange = null;
        gameScreen.InventoryChange = null;
        if (hotSlot is { })
            intentSender.Send(new HotbarSwapIntent(default, hotSlot.Value, (byte)slot));

        var invSlot = inventory.DragOrigin;
        if (invSlot is { })
            intentSender.Send(new HotbarAddIntent(default, (byte)slot, SlotType.Item, invSlot.Value));
    }

    private void OnSlotRightClick(int slot)
    {
        intentSender.Send(new HotbarAddIntent(default, (byte)slot, default, 0));
    }

    private void OnSlotDoubleClick(int slot)
    {
        var hbSlot = context.LocalPlayer.GetHotbar()?.Slots[slot];
        if (hbSlot is HotbarSlot { Slot: > 0 })
        {
            intentSender.Send(new HotbarUseIntent(default, (byte)slot));
            uiContext.Registry["Drop"].Visible = false;
        }
    }

    private void OnSlotHoverEnter(int slot)
    {
        var hotbarSlot = context.LocalPlayer.GetHotbar()?.Slots[slot];
        if (hotbarSlot is HotbarSlot { Slot: > 0, Type: SlotType.Item } h)
        {
            var item = catalog.Items.Get(context.LocalPlayer.GetInventory()?.Slots[h.Slot] is ItemSlot s ? s.ItemId : Guid.Empty);
            if (item == null) return;
            var panelRect = uiContext.Registry["HotbarPanel"].LastBoundingRect;
            tooltip.Show(item.Id, new Point(panelRect.X, panelRect.Y + 42));
        }
    }

    private void OnPostDraw()
    {
        if (context.LocalPlayer.Entity == null) return;
        var hotbar = context.World.Get<HotbarState>(context.LocalPlayer.Entity.Value);
        if (hotbar == null) return;

        for (var i = 0; i < Grid.TotalSlots; i++)
        {
            var rect = Grid.GetSlotRect(i);
            var hotbarSlot = hotbar.Slots[i];
            if (hotbarSlot is HotbarSlot { Slot: > 0, Type: SlotType.Item } h)
            {
                var itemId = context.LocalPlayer.GetInventory()?.Slots[h.Slot] is ItemSlot s ? s.ItemId : Guid.Empty;
                if (catalog.Items.Get(itemId) is { } item)
                    itemRenderer.DrawItem(item, 1, new Point(rect.X, rect.Y));
            }
        }
    }
}
