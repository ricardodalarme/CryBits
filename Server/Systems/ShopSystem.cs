using System;
using System.Drawing;
using CryBits.Entities.Shop;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

/// <summary>System that owns all shop lifecycle logic.</summary>
internal static class ShopSystem
{
    /// <summary>Opens <paramref name="shop"/> for <paramref name="player"/> and notifies the client.</summary>
    public static void Open(Player player, Shop shop)
    {
        player.Shop = shop;
        ShopSender.ShopOpen(player, shop);
    }

    /// <summary>Closes the active shop session for <paramref name="player"/> and notifies the client.</summary>
    public static void Leave(Player player)
    {
        if (player.Shop == null) return;

        player.Shop = null;
        ShopSender.ShopOpen(player, null);
    }

    /// <summary>Purchases a shop item for <paramref name="player"/>.</summary>
    internal static void Buy(Player player, short shopSoldIndex)
    {
        var shopSold = player.Shop.Sold[shopSoldIndex];
        var inventorySlot = player.FindInventory(player.Shop.Currency);

        if (inventorySlot == null || inventorySlot.Amount < shopSold.Price)
        {
            ChatSender.Message(player, "You don't have enough money to buy the item.", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && inventorySlot.Amount > shopSold.Price)
        {
            ChatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        InventorySystem.TakeItem(player, inventorySlot, shopSold.Price);
        InventorySystem.GiveItem(player, shopSold.Item, shopSold.Amount);
        ChatSender.Message(player, "You bought " + shopSold.Price + "x " + shopSold.Item.Name + ".", Color.Green);
    }

    /// <summary>Sells an inventory item back to the shop for <paramref name="player"/>.</summary>
    internal static void Sell(Player player, byte inventorySlotIndex, short amount)
    {
        amount = Math.Min(amount, player.Inventory[inventorySlotIndex].Amount);
        var buy = player.Shop.FindBought(player.Inventory[inventorySlotIndex].Item);

        if (buy == null)
        {
            ChatSender.Message(player, "The store doesn't sell this item", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && player.Inventory[inventorySlotIndex].Amount > amount)
        {
            ChatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        ChatSender.Message(player,
            "You sold " + player.Inventory[inventorySlotIndex].Item.Name + "x " + amount + " for .", Color.Green);
        InventorySystem.TakeItem(player, player.Inventory[inventorySlotIndex], amount);
        InventorySystem.GiveItem(player, player.Shop.Currency, (short)(buy.Price * amount));
    }
}
