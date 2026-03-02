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

        // Set information panel position and id according to the hovered slot
        if (HotbarView.CurrentSlot >= 0)
        {
            position = HotbarView.Panel.Position + new Size(0, 42);
            CurrentId = Player.Me.Inventory[Player.Me.Hotbar[HotbarView.CurrentSlot].Slot].Item.GetId();
        }
        else if (InventoryView.CurrentSlot > 0)
        {
            position = InventoryView.Panel.Position + new Size(-186, 3);
            CurrentId = Player.Me.Inventory[InventoryView.CurrentSlot].Item.GetId();
        }
        else if (CharacterView.CurrentSlot >= 0)
        {
            position = CharacterView.Panel.Position + new Size(-186, 5);
            CurrentId = Player.Me.Equipment[CharacterView.CurrentSlot].GetId();
        }
        else if (ShopView.CurrentSlot >= 0 && ShopView.CurrentSlot < ShopView.OpenedShop.Sold.Count)
        {
            position = new Point(ShopView.Panel.Position.X - 186, ShopView.Panel.Position.Y + 5);
            CurrentId = ShopView.OpenedShop.Sold[ShopView.CurrentSlot].Item.GetId();
        }
        else CurrentId = Guid.Empty;

        Panel.Visible = !position.IsEmpty && CurrentId != Guid.Empty;
        Panel.Position = position;
    }
}
