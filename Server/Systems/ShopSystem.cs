using System;
using System.Drawing;
using CryBits.Entities.Shop;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

/// <summary>System that owns all shop lifecycle logic.</summary>
internal static class ShopSystem
{
    private static CryBits.Server.ECS.World World => ServerContext.Instance.World;

    /// <summary>Opens <paramref name="shop"/> for <paramref name="player"/> and notifies the client.</summary>
    public static void Open(Player player, Shop shop)
    {
        World.Add(player.EntityId, new ShopComponent { Active = shop });
        ShopSender.ShopOpen(player, shop);
    }

    /// <summary>Closes the active shop session for <paramref name="player"/> and notifies the client.</summary>
    public static void Leave(Player player)
    {
        if (!player.Has<ShopComponent>()) return;

        World.Remove<ShopComponent>(player.EntityId);
        ShopSender.ShopOpen(player, null);
    }

    /// <summary>Purchases a shop item for <paramref name="player"/>.</summary>
    internal static void Buy(Player player, short shopSoldIndex)
    {
        var shop      = player.Get<ShopComponent>().Active;
        var shopSold  = shop.Sold[shopSoldIndex];
        var invSlot   = player.FindInventory(shop.Currency);

        if (invSlot == null || invSlot.Amount < shopSold.Price)
        {
            ChatSender.Message(player, "You don't have enough money to buy the item.", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && invSlot.Amount > shopSold.Price)
        {
            ChatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        InventorySystem.TakeItem(player, invSlot, shopSold.Price);
        InventorySystem.GiveItem(player, shopSold.Item, shopSold.Amount);
        ChatSender.Message(player, "You bought " + shopSold.Price + "x " + shopSold.Item.Name + ".", Color.Green);
    }

    /// <summary>Sells an inventory item back to the shop for <paramref name="player"/>.</summary>
    internal static void Sell(Player player, byte inventorySlotIndex, short amount)
    {
        var shop = player.Get<ShopComponent>().Active;
        var inv  = player.Get<InventoryComponent>();
        var slot = inv.Slots[inventorySlotIndex];

        amount = Math.Min(amount, slot.Amount);
        var buy = shop.FindBought(slot.Item);

        if (buy == null)
        {
            ChatSender.Message(player, "The store doesn't sell this item", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && slot.Amount > amount)
        {
            ChatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        ChatSender.Message(player,
            "You sold " + slot.Item!.Name + "x " + amount + " for .", Color.Green);
        InventorySystem.TakeItem(player, slot, amount);
        InventorySystem.GiveItem(player, shop.Currency, (short)(buy.Price * amount));
    }
}
