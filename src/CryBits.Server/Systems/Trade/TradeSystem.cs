using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Slots;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Inventory;
using CryBits.Server.World;
using System;
using System.Drawing;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;

namespace CryBits.Server.Systems.Trade;

internal sealed class TradeSystem(
    TradeSender tradeSender,
    ChatSender chatSender,
    InventorySystem inventorySystem,
    PlayerSender playerSender,
    DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static TradeSystem Instance { get; } = new(
        TradeSender.Instance,
        ChatSender.Instance,
        InventorySystem.Instance,
        PlayerSender.Instance,
        DefinitionCatalog.Instance);

    internal void Invite(Player player, string targetName)
    {
        var invited = GameWorld.Current.FindPlayer(targetName);

        if (invited == null)
        {
            chatSender.Message(player, "The player isn't connected.", Color.White);
            return;
        }

        if (invited == player)
        {
            chatSender.Message(player, "You can't be invited.", Color.White);
            return;
        }

        if (invited.Trade != null)
        {
            chatSender.Message(player, "The player is already part of a trade.", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(invited.TradeRequest))
        {
            chatSender.Message(player, "The player is analyzing an invitation of another trade.", Color.White);
            return;
        }

        if (player.Shop != null)
        {
            chatSender.Message(player, "You can't start a trade while in the shop.", Color.White);
            return;
        }

        if (invited.Shop != null)
        {
            chatSender.Message(player, "The player is in the shop.", Color.White);
            return;
        }

        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            chatSender.Message(player, "You need to be close to the player to start trade.", Color.White);
            return;
        }

        invited.TradeRequest = player.Name;
        tradeSender.TradeInvitation(invited, player.Name);
    }

    internal void Accept(Player player)
    {
        var invited = GameWorld.Current.FindPlayer(player.TradeRequest);

        if (player.Trade != null)
        {
            chatSender.Message(player, "You are already part of a trade.", Color.White);
            return;
        }

        if (invited == null)
        {
            chatSender.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }

        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            chatSender.Message(player, "You need to be close to the player to accept the trade.", Color.White);
            return;
        }

        if (invited.Shop != null)
        {
            chatSender.Message(player, "Who invited you is in the shop.", Color.White);
            return;
        }

        player.Trade = invited;
        invited.Trade = player;
        chatSender.Message(player, "You have accepted " + invited.Name + "'s trade request.", Color.White);
        chatSender.Message(invited, player.Name + " has accepted your trade request.", Color.White);

        player.TradeRequest = string.Empty;
        player.TradeOffer = new TradeSlot[MaxInventory];
        invited.TradeOffer = new TradeSlot[MaxInventory];

        tradeSender.Trade(player, true);
        tradeSender.Trade(invited, true);
    }

    internal void Decline(Player player)
    {
        var invited = GameWorld.Current.FindPlayer(player.TradeRequest);
        if (invited != null) chatSender.Message(invited, player.Name + " decline the trade.", Color.White);
        player.TradeRequest = string.Empty;
    }

    public void Leave(Player player)
    {
        if (player.Trade == null) return;

        player.Trade.Trade = null;
        tradeSender.Trade(player.Trade, false);
        player.Trade = null;
        tradeSender.Trade(player, false);
    }

    internal void Offer(Player player, short slot, short inventorySlot, short amount)
    {
        amount = Math.Min(amount, player.Inventory[inventorySlot].Amount);

        if (inventorySlot != 0)
        {
            for (byte i = 0; i < MaxInventory; i++)
                if (player.TradeOffer[i].SlotNum == inventorySlot)
                    return;

            player.TradeOffer[slot].SlotNum = inventorySlot;
            player.TradeOffer[slot].Amount = amount;
        }
        else
            player.TradeOffer[slot] = new TradeSlot();

        tradeSender.TradeOffer(player);
        tradeSender.TradeOffer(player.Trade, false);
    }

    internal void OfferState(Player player, TradeStatus state)
    {
        var invited = player.Trade;

        switch (state)
        {
            case TradeStatus.Accepted:
                if (player.TotalTradeItems > invited.TotalInventoryFree)
                {
                    chatSender.Message(invited,
                        invited.Name + " don't have enough space in their inventory to do this trade.", Color.Red);
                    break;
                }

                if (invited.TotalTradeItems > player.TotalInventoryFree)
                {
                    chatSender.Message(invited, "You don't have enough space in your inventory to do this trade.",
                        Color.Red);
                    break;
                }

                chatSender.Message(invited, "The offer was accepted.", Color.Green);

                ItemSlot[] yourInventory = (ItemSlot[])player.Inventory.Clone(),
                    theirInventory = (ItemSlot[])invited.Inventory.Clone();

                var to = player;
                for (byte j = 0; j < 2; j++, to = to == player ? invited : player)
                    for (byte i = 0; i < MaxInventory; i++)
                        inventorySystem.TakeItem(to, to.Inventory[to.TradeOffer[i].SlotNum], to.TradeOffer[i].Amount);

                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (player.TradeOffer[i].SlotNum > 0)
                        inventorySystem.GiveItem(invited, _catalog.Items.Get(yourInventory[player.TradeOffer[i].SlotNum].ItemId),
                            player.TradeOffer[i].Amount);
                    if (invited.TradeOffer[i].SlotNum > 0)
                        inventorySystem.GiveItem(player, _catalog.Items.Get(theirInventory[invited.TradeOffer[i].SlotNum].ItemId),
                            invited.TradeOffer[i].Amount);
                }

                playerSender.PlayerInventory(player);
                playerSender.PlayerInventory(invited);

                player.TradeOffer = new TradeSlot[MaxInventory];
                invited.TradeOffer = new TradeSlot[MaxInventory];
                tradeSender.TradeOffer(invited);
                tradeSender.TradeOffer(invited, false);
                break;

            case TradeStatus.Declined:
                chatSender.Message(invited, "The offer was declined.", Color.Red);
                break;

            case TradeStatus.Waiting:
                chatSender.Message(invited, player.Name + " send you a offer.", Color.White);
                break;
        }

        tradeSender.TradeState(invited, state);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case PlayerStartedMovingEvent e:
                    {
                        var player = world.FindPlayer(e.PlayerId);
                        if (player != null) Leave(player);
                        break;
                    }
                case PlayerWarpedEvent e:
                    {
                        var player = world.FindPlayer(e.PlayerId);
                        if (player != null) Leave(player);
                        break;
                    }
                case PlayerDisconnectedEvent e:
                    {
                        var player = world.FindPlayer(e.PlayerId);
                        if (player != null) Leave(player);
                        break;
                    }
            }
        }
    }
}
