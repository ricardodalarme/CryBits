using System;
using System.Drawing;
using CryBits.Entities.Shop;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

/// <summary>System that owns all shop lifecycle logic.</summary>
internal sealed class ShopSystem(
    InventorySystem inventorySystem,
    ShopSender shopSender,
    ChatSender chatSender)
{
    public static ShopSystem Instance { get; } = new(
        InventorySystem.Instance,
        ShopSender.Instance,
        ChatSender.Instance);

    private readonly InventorySystem _inventorySystem = inventorySystem;
    private readonly ShopSender _shopSender = shopSender;
    private readonly ChatSender _chatSender = chatSender;

    /// <summary>Opens <paramref name="shop"/> for <paramref name="player"/> and notifies the client.</summary>
    public void Open(Player player, Shop shop)
    {
        player.Shop = shop;
        _shopSender.ShopOpen(player, shop);
    }

    /// <summary>Closes the active shop session for <paramref name="player"/> and notifies the client.</summary>
    public void Leave(Player player)
    {
        if (player.Shop == null) return;

        player.Shop = null;
        _shopSender.ShopOpen(player, null);
    }

    /// <summary>Purchases a shop item for <paramref name="player"/>.</summary>
    internal void Buy(Player player, short shopSoldIndex)
    {
        var shopSold = player.Shop.Sold[shopSoldIndex];
        var inventorySlot = player.FindInventory(player.Shop.Currency);

        if (inventorySlot == null || inventorySlot.Amount < shopSold.Price)
        {
            _chatSender.Message(player, "You don't have enough money to buy the item.", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && inventorySlot.Amount > shopSold.Price)
        {
            _chatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        _inventorySystem.TakeItem(player, inventorySlot, shopSold.Price);
        _inventorySystem.GiveItem(player, shopSold.Item, shopSold.Amount);
        _chatSender.Message(player, "You bought " + shopSold.Price + "x " + shopSold.Item.Name + ".", Color.Green);
    }

    /// <summary>Sells an inventory item back to the shop for <paramref name="player"/>.</summary>
    internal void Sell(Player player, byte inventorySlotIndex, short amount)
    {
        amount = Math.Min(amount, player.Inventory[inventorySlotIndex].Amount);
        var buy = player.Shop.FindBought(player.Inventory[inventorySlotIndex].Item);

        if (buy == null)
        {
            _chatSender.Message(player, "The store doesn't sell this item", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && player.Inventory[inventorySlotIndex].Amount > amount)
        {
            _chatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        _chatSender.Message(player,
            "You sold " + player.Inventory[inventorySlotIndex].Item.Name + "x " + amount + " for .", Color.Green);
        _inventorySystem.TakeItem(player, player.Inventory[inventorySlotIndex], amount);
        _inventorySystem.GiveItem(player, player.Shop.Currency, (short)(buy.Price * amount));
    }
}
