using System;
using System.Drawing;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Handlers;

internal static class ShopHandler
{
    internal static void ShopBuy(Player player, NetDataReader data)
    {
        var shopSold = player.Shop.Sold[data.GetShort()];
        var inventorySlot = player.FindInventory(player.Shop.Currency);

        // Verifica se o jogador tem dinheiro
        if (inventorySlot == null || inventorySlot.Amount < shopSold.Price)
        {
            ChatSender.Message(player, "You don't have enough money to buy the item.", Color.Red);
            return;
        }

        // Verifica se há espaço no inventário
        if (player.TotalInventoryFree == 0 && inventorySlot.Amount > shopSold.Price)
        {
            ChatSender.Message(player, "You  don't have space in your bag.", Color.Red);
            return;
        }

        // Realiza a compra do item
        InventorySystem.TakeItem(player, inventorySlot, shopSold.Price);
        InventorySystem.GiveItem(player, shopSold.Item, shopSold.Amount);
        ChatSender.Message(player, "You bought " + shopSold.Price + "x " + shopSold.Item.Name + ".", Color.Green);
    }

    internal static void ShopSell(Player player, NetDataReader data)
    {
        var inventorySlot = data.GetByte();
        var amount = Math.Min(data.GetShort(), player.Inventory[inventorySlot].Amount);
        var buy = player.Shop.FindBought(player.Inventory[inventorySlot].Item);

        // Verifica se a loja vende o item
        if (buy == null)
        {
            ChatSender.Message(player, "The store doesn't sell this item", Color.Red);
            return;
        }

        // Verifica se há espaço no inventário
        if (player.TotalInventoryFree == 0 && player.Inventory[inventorySlot].Amount > amount)
        {
            ChatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        // Realiza a venda do item
        ChatSender.Message(player, "You sold " + player.Inventory[inventorySlot].Item.Name + "x " + amount + "for .",
            Color.Green);
        InventorySystem.TakeItem(player, player.Inventory[inventorySlot], amount);
        InventorySystem.GiveItem(player, player.Shop.Currency, (short)(buy.Price * amount));
    }

    internal static void ShopClose(Player player)
    {
        ShopSystem.Leave(player);
    }
}
