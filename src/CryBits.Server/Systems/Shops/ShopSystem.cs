using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.Events;
using CryBits.Server.Systems.Inventory;
using CryBits.Server.World;
using System;
using System.Drawing;

namespace CryBits.Server.Systems.Shops;

internal sealed class ShopSystem(
    InventorySystem inventorySystem,
    ShopSender shopSender,
    ChatSender chatSender) : ISimulationSystem
{
    public static ShopSystem Instance { get; } = new(
        InventorySystem.Instance,
        ShopSender.Instance,
        ChatSender.Instance);

    public void Open(Player player, Shop shop)
    {
        player.Shop = shop;
        shopSender.ShopOpen(player, shop);
    }

    public void Leave(Player player)
    {
        if (player.Shop == null) return;

        player.Shop = null;
        shopSender.ShopOpen(player, null);
    }

    internal void Buy(Player player, short shopSoldIndex)
    {
        var shopSold = player.Shop.Sold[shopSoldIndex];
        var inventorySlot = player.FindInventory(player.Shop.Currency);

        if (inventorySlot == null || inventorySlot.Amount < shopSold.Price)
        {
            chatSender.Message(player, "You don't have enough money to buy the item.", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && inventorySlot.Amount > shopSold.Price)
        {
            chatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        inventorySystem.TakeItem(player, inventorySlot, shopSold.Price);
        inventorySystem.GiveItem(player, shopSold.Item, shopSold.Amount);
        chatSender.Message(player, "You bought " + shopSold.Price + "x " + shopSold.Item.Name + ".", Color.Green);
    }

    internal void Sell(Player player, byte inventorySlotIndex, short amount)
    {
        amount = Math.Min(amount, player.Inventory[inventorySlotIndex].Amount);
        var buy = player.Shop.FindBought(player.Inventory[inventorySlotIndex].Item);

        if (buy == null)
        {
            chatSender.Message(player, "The store doesn't sell this item", Color.Red);
            return;
        }

        if (player.TotalInventoryFree == 0 && player.Inventory[inventorySlotIndex].Amount > amount)
        {
            chatSender.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        chatSender.Message(player,
            "You sold " + player.Inventory[inventorySlotIndex].Item.Name + "x " + amount + " for .", Color.Green);
        inventorySystem.TakeItem(player, player.Inventory[inventorySlotIndex], amount);
        inventorySystem.GiveItem(player, player.Shop.Currency, (short)(buy.Price * amount));
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case PlayerStartedMovingEvent e:
                    Leave(e.Player);
                    break;
                case PlayerWarpedEvent e:
                    Leave(e.Player);
                    break;
                case NpcAttackedEvent e:
                    if (e.Npc.Data.Behaviour == Behaviour.ShopKeeper)
                        Open(e.Attacker, e.Npc.Data.Shop);
                    break;
            }
        }
    }
}
