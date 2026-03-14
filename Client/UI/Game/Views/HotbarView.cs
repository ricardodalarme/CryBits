using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Enums;
using SFML.Window;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class HotbarView(PlayerSender playerSender, ItemRenderer itemRenderer, GameContext context) : IView
{
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
        var hotbarSlot = context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot?.Slot > 0 && hotbarSlot.Type == SlotType.Item)
            itemRenderer.DrawItem(context.LocalPlayer.GetInventory().Slots[hotbarSlot.Slot]?.Item, 1, pos);
    }

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        var hotbarSlot = context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot == null || hotbarSlot.Slot == 0) return;

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
        if (GameScreen.HotbarChange != null) playerSender.HotbarChange(GameScreen.HotbarChange.Value, slot);
        if (GameScreen.InventoryChange != null) playerSender.HotbarAdd(slot, (byte)SlotType.Item, GameScreen.InventoryChange.Value);
    }

    private void OnGridMouseDoubleClick(MouseButtonEventArgs e, short slot)
    {
        var hotbarSlot = context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot == null || hotbarSlot.Slot <= 0) return;

        // Use item from hotbar
        playerSender.HotbarUse((byte)slot);
        DropItemView.Panel.Visible = false;
    }

    private void OnGridSlotHover(short slot)
    {
        var hotbarSlot = context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot == null || hotbarSlot.Slot <= 0 || hotbarSlot.Type != SlotType.Item) return;
        var item = context.LocalPlayer.GetInventory().Slots[hotbarSlot.Slot]?.Item;
        if (item == null) return;
        InformationView.Show(item.Id, Panel.Position + new Size(0, 42));
    }

    private static void OnGridSlotLeave(short slot) => InformationView.Hide();
}
