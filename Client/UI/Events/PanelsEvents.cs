using System;
using System.Drawing;
using CryBits.Client.ECS;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using SFML.Window;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.UI.Events;

internal static class PanelsEvents
{
    // Temporary state
    public static byte CreateCharacterClass = 0;
    public static byte CreateCharacterTex = 0;
    public static int SelectCharacter = 1;
    public static Guid InformationId;
    public static short DropSlot;
    public static string PartyInvitation;
    public static string TradeInvitation;
    public static short TradeInventorySlot = -1;
    public static Shop ShopOpen;
    public static short ShopInventorySlot;
    public static short HotbarChange;
    public static short InventoryChange;
    public static TempCharacter[] Characters;

    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    public static void Bind()
    {
        Panels.MenuCharacter.OnMouseDown += Equipment_MouseDown;
        Panels.MenuInventory.OnMouseDown += Inventory_MouseDown;
        Panels.MenuInventory.OnMouseUp += Inventory_MouseUp;
        Panels.MenuInventory.OnMouseDoubleClick += Inventory_MouseDoubleClick;
        Panels.Hotbar.OnMouseDown += Hotbar_MouseDown;
        Panels.Hotbar.OnMouseUp += Hotbar_MouseUp;
        Panels.Hotbar.OnMouseDoubleClick += Hotbar_MouseDoubleClick;
        Panels.Trade.OnMouseDown += Trade_MouseDown;
        Panels.Trade.OnMouseUp += Trade_MouseUp;
        Panels.Trade.OnMouseDoubleClick += Trade_MouseDoubleClick;
    }

    public static void MenuClose()
    {
        Panels.Connect.Visible = false;
        Panels.Register.Visible = false;
        Panels.Options.Visible = false;
        Panels.SelectCharacter.Visible = false;
        Panels.CreateCharacter.Visible = false;
    }

    public static short Slot(Panel panel, byte offX, byte offY, byte lines, byte columns, byte grid = 32, byte gap = 4)
    {
        var size = grid + gap;
        var start = panel.Position + new Size(offX, offY);
        var slot = new Point((Window.Mouse.X - start.X) / size, (Window.Mouse.Y - start.Y) / size);

        // Check whether the mouse is over the slot
        if (slot.Y < 0 || slot.X < 0 || slot.X >= columns || slot.Y >= lines) return -1;
        if (!IsAbove(new Rectangle(start.X + slot.X * size, start.Y + slot.Y * size, grid, grid))) return -1;
        if (!panel.Visible) return -1;

        // Return slot index
        return (short)(slot.Y * columns + slot.X);
    }

    // Slot indices under the mouse cursor
    public static short InventorySlot => Slot(Panels.MenuInventory, 7, 29, 6, 5);
    public static short HotbarSlot => Slot(Panels.Hotbar, 8, 6, 1, 10);
    public static short TradeSlot => Slot(Panels.Trade, 7, 50, 6, 5);
    public static short ShopSlot => Slot(Panels.Shop, 7, 50, 4, 7);
    public static short EquipmentSlot => Slot(Panels.MenuCharacter, 7, 248, 1, 5);

    // ECS helpers — safe accessors for local player components.
    private static GameContext Ctx => GameContext.Instance;
    private static int LocalId => Ctx.GetLocalPlayer();
    private static T? Local<T>() where T : class, IComponent =>
        LocalId >= 0 && Ctx.World.TryGet<T>(LocalId, out var c) ? c : null;

    public static void Inventory_MouseDown(MouseButtonEventArgs e)
    {
        var slot = InventorySlot;

        var inv = Local<InventoryComponent>();
        if (slot == -1) return;
        if (inv?.Slots[slot]?.Item == null) return;

        switch (e.Button)
        {
            case Mouse.Button.Right:
                {
                    if (inv.Slots[slot]!.Item!.Bind != BindOn.Pickup)
                        // Sell the item if shop is open
                        if (Panels.Shop.Visible)
                        {
                            if (inv.Slots[slot]!.Amount != 1)
                            {
                                ShopInventorySlot = slot;
                                TextBoxes.ShopSellAmount.Text = string.Empty;
                                Panels.ShopSell.Visible = true;
                            }
                            else ShopSender.ShopSell(slot, 1);
                        }
                        // Otherwise drop the item
                        else if (!Panels.Trade.Visible)
                            if (inv.Slots[slot]!.Amount != 1)
                            {
                                DropSlot = slot;
                                TextBoxes.DropAmount.Text = string.Empty;
                                Panels.Drop.Visible = true;
                            }
                            else PlayerSender.DropItem(slot, 1);

                    break;
                }
            // Select the item (start drag)
            case Mouse.Button.Left:
                InventoryChange = slot;
                break;
        }
    }

    public static void Equipment_MouseDown(MouseButtonEventArgs e)
    {
        var panelPosition = Panels.MenuCharacter.Position;

        var pd = Local<PlayerDataComponent>();
        for (byte i = 0; i < (byte)Equipment.Count; i++)
            if (IsAbove(new Rectangle(panelPosition.X + 7 + i * 36, panelPosition.Y + 247, 32, 32)))
                // Remove equipment on right-click
                if (e.Button == Mouse.Button.Right)
                    if (pd?.EquippedItems[i]?.Bind != BindOn.Equip)
                    {
                        PlayerSender.EquipmentRemove(i);
                        return;
                    }
    }

    public static void Hotbar_MouseDown(MouseButtonEventArgs e)
    {
        var slot = HotbarSlot;

        if (slot < 0) return;
        if ((Local<HotbarComponent>()?.Slots[slot].Slot ?? 0) == 0) return;

        switch (e.Button)
        {
            // Drop or use hotbar slot (right-click)
            case Mouse.Button.Right:
                PlayerSender.HotbarAdd(slot, 0, 0);
                break;
            // Select the hotbar slot (start drag)
            case Mouse.Button.Left:
                HotbarChange = slot;
                break;
        }
    }

    public static void Trade_MouseDown(MouseButtonEventArgs e)
    {
        var slot = TradeSlot;

        if (!Panels.Trade.Visible) return;
        if (slot == -1) return;
        if (Local<TradeComponent>()?.Offer?[slot]?.Item == null) return;

        if (e.Button == Mouse.Button.Right) TradeSender.TradeOffer(slot, 0);
    }

    public static void CheckInformation()
    {
        var position = new Point();

        // Set information panel position and id according to the hovered slot
        if (HotbarSlot >= 0)
        {
            position = Panels.Hotbar.Position + new Size(0, 42);
            var hotbar = Local<HotbarComponent>();
            var inv = Local<InventoryComponent>();
            var invSlot = hotbar?.Slots[HotbarSlot].Slot ?? 0;
            InformationId = inv?.Slots[invSlot]?.Item?.GetId() ?? Guid.Empty;
        }
        else if (InventorySlot > 0)
        {
            position = Panels.MenuInventory.Position + new Size(-186, 3);
            InformationId = Local<InventoryComponent>()?.Slots[InventorySlot]?.Item?.GetId() ?? Guid.Empty;
        }
        else if (EquipmentSlot >= 0)
        {
            position = Panels.MenuCharacter.Position + new Size(-186, 5);
            InformationId = Local<PlayerDataComponent>()?.EquippedItems[EquipmentSlot]?.GetId() ?? Guid.Empty;
        }
        else if (ShopSlot >= 0 && ShopSlot < ShopOpen.Sold.Count)
        {
            position = new Point(Panels.Shop.Position.X - 186, Panels.Shop.Position.Y + 5);
            InformationId = ShopOpen.Sold[ShopSlot].Item.GetId();
        }
        else InformationId = Guid.Empty;

        Panels.Information.Visible = !position.IsEmpty && InformationId != Guid.Empty;
        Panels.Information.Position = position;
    }

    public static void Inventory_MouseUp()
    {
        // Return early when no valid slot or no change pending.
        if (InventorySlot == 0) return;
        if (InventoryChange == 0) return;

        // Send inventory slot change to server.
        PlayerSender.InventoryChange(InventoryChange, InventorySlot);
    }

    public static void Hotbar_MouseUp()
    {
        // Change hotbar slot
        if (HotbarSlot < 0) return;
        if (HotbarChange >= 0) PlayerSender.HotbarChange(HotbarChange, HotbarSlot);
        if (InventoryChange > 0) PlayerSender.HotbarAdd(HotbarSlot, (byte)SlotType.Item, InventoryChange);
    }

    public static void Trade_MouseUp()
    {
        if (InventoryChange <= 0) return;

        // Add item to trade
        if (Local<InventoryComponent>()?.Slots[InventoryChange]?.Amount == 1)
            TradeSender.TradeOffer(TradeSlot, InventoryChange);
        else
        {
            TradeInventorySlot = InventoryChange;
            TextBoxes.TradeAmount.Text = string.Empty;
            Panels.TradeAmount.Visible = true;
        }
    }

    public static void Inventory_MouseDoubleClick(MouseButtonEventArgs e)
    {
        var slot = InventorySlot;
        if (slot <= 0) return;
        if (Local<InventoryComponent>()?.Slots[slot]?.Item == null) return;

        // Use item
        PlayerSender.InventoryUse((byte)slot);
    }

    public static void Hotbar_MouseDoubleClick(MouseButtonEventArgs e)
    {
        var slot = HotbarSlot;
        if (slot <= 0) return;
        if ((Local<HotbarComponent>()?.Slots[slot].Slot ?? 0) <= 0) return;

        // Use item from hotbar
        PlayerSender.HotbarUse((byte)slot);
    }

    public static void Trade_MouseDoubleClick(MouseButtonEventArgs e)
    {
        var slot = ShopSlot;
        if (slot < 0) return;
        if (ShopOpen == null) return;

        // Purchase shop item.
        ShopSender.ShopBuy((byte)slot);
    }
}
