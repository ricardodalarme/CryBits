using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using SFML.Window;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class HotbarView(IntentSender intentSender, ItemRenderer itemRenderer, GameContext context, DefinitionCatalog catalog) : IView
{
    private readonly DefinitionCatalog _catalog = catalog;
    internal static Panel Panel => Tools.Panels["Hotbar"];
    private static SlotGrid Grid => Tools.SlotGrids["Hotbar_Grid"];

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
        if (context.LocalPlayer.Entity == null) return;
        var hotbar = context.World.Get<HotbarState>(context.LocalPlayer.Entity.Value);
        if (hotbar == null) return;

        var hotbarSlot = hotbar.Slots[slot];
        if (hotbarSlot is HotbarSlot { Slot: > 0, Type: SlotType.Item } h)
        {
            var itemId = context.LocalPlayer.GetInventory()?.Slots[h.Slot] is ItemSlot s ? s.ItemId : Guid.Empty;
            if (_catalog.Items.Get(itemId) is { } item) itemRenderer.DrawItem(item, 1, pos);
        }
    }

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        var hotbarSlot = context.LocalPlayer.GetHotbar()?.Slots[slot];
        if (hotbarSlot is not HotbarSlot { Slot: not 0 }) return;

        switch (e.Button)
        {
            case Mouse.Button.Right:
                intentSender.Send(new HotbarAddIntent(default, slot, default, 0));
                break;
            case Mouse.Button.Left:
                GameScreen.HotbarChange = slot;
                break;
        }
    }

    private void OnGridMouseUp(short slot)
    {
        if (GameScreen.HotbarChange >= 0) intentSender.Send(new HotbarSwapIntent(default, GameScreen.HotbarChange, slot));
        if (GameScreen.InventoryChange > 0) intentSender.Send(new HotbarAddIntent(default, slot, SlotType.Item, GameScreen.InventoryChange));
    }

    private void OnGridMouseDoubleClick(MouseButtonEventArgs e, short slot)
    {
        var hotbarSlot = context.LocalPlayer.GetHotbar()?.Slots[slot];
        if (hotbarSlot is not HotbarSlot { Slot: > 0 }) return;

        intentSender.Send(new HotbarUseIntent(default, (byte)slot));
        DropItemView.Panel.Visible = false;
    }

    private void OnGridSlotHover(short slot)
    {
        var hotbarSlot = context.LocalPlayer.GetHotbar()?.Slots[slot];
        if (hotbarSlot is HotbarSlot { Slot: > 0, Type: SlotType.Item } h)
        {
            var item = _catalog.Items.Get(context.LocalPlayer.GetInventory()?.Slots[h.Slot] is ItemSlot s ? s.ItemId : Guid.Empty);
            if (item == null) return;
            InformationView.Show(item.Id, Panel.Position + new Size(0, 42));
        }
    }

    private static void OnGridSlotLeave(short slot) => InformationView.Hide();
}
