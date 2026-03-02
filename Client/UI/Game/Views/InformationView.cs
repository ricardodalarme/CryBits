using System;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Extensions;

namespace CryBits.Client.UI.Game.Views;

internal class InformationView
{
    internal static Panel Panel => Tools.Panels["Information"];

    public static Guid CurrentId;

    public static void CheckInformation()
    {
        var position = new Point();

        var hotbarSlot = Tools.SlotGrids["Hotbar_Grid"].GetSlotIndex();
        var inventorySlot = Tools.SlotGrids["Inventory_Grid"].GetSlotIndex();
        var equipmentSlot = Tools.SlotGrids["Equipment_Grid"].GetSlotIndex();
        var shopSlot = Tools.SlotGrids["Shop_Grid"].GetSlotIndex();

        // Set information panel position and id according to the hovered slot
        if (hotbarSlot >= 0)
        {
            position = HotbarView.Panel.Position + new Size(0, 42);
            CurrentId = Player.Me.Inventory[Player.Me.Hotbar[hotbarSlot].Slot].Item.GetId();
        }
        else if (inventorySlot > 0)
        {
            position = InventoryView.Panel.Position + new Size(-186, 3);
            CurrentId = Player.Me.Inventory[inventorySlot].Item.GetId();
        }
        else if (equipmentSlot >= 0)
        {
            position = CharacterView.Panel.Position + new Size(-186, 5);
            CurrentId = Player.Me.Equipment[equipmentSlot].GetId();
        }
        else if (shopSlot >= 0 && shopSlot < ShopView.OpenedShop.Sold.Count)
        {
            position = new Point(ShopView.Panel.Position.X - 186, ShopView.Panel.Position.Y + 5);
            CurrentId = ShopView.OpenedShop.Sold[shopSlot].Item.GetId();
        }
        else CurrentId = Guid.Empty;

        Panel.Visible = !position.IsEmpty && CurrentId != Guid.Empty;
        Panel.Position = position;
    }
}
