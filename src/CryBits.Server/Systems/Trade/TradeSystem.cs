using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Slots;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using System.Linq;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.State;
using CryBits.Server.Simulation.State.Components;
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

    internal void Invite(EntityId entityId, string targetName)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>()!;
        var shop = e.Get<ShopState>();

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            chatSender.Message(entityId, "The player isn't connected.", Color.White);
            return;
        }

        if (invitedId.Value == entityId)
        {
            chatSender.Message(entityId, "You can't be invited.", Color.White);
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedTrade = invitedE.Get<TradeState>()!;
        var invitedPos = invitedE.Get<Position>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (invitedTrade.Partner != null)
        {
            chatSender.Message(entityId, "The player is already part of a trade.", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(invitedTrade.Request))
        {
            chatSender.Message(entityId, "The player is analyzing an invitation of another trade.", Color.White);
            return;
        }

        if (shop?.ShopId != null)
        {
            chatSender.Message(entityId, "You can't start a trade while in the shop.", Color.White);
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            chatSender.Message(entityId, "The player is in the shop.", Color.White);
            return;
        }

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            chatSender.Message(entityId, "You need to be close to the player to start trade.", Color.White);
            return;
        }

        invitedTrade.Request = appearance.Name;
        tradeSender.TradeInvitation(invitedId.Value, appearance.Name);
    }

    internal void Accept(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>()!;
        var shop = e.Get<ShopState>();

        var invitedId = world.FindPlayer(trade.Request);

        if (trade.Partner != null)
        {
            chatSender.Message(entityId, "You are already part of a trade.", Color.White);
            return;
        }

        if (invitedId == null)
        {
            chatSender.Message(entityId, "Who invited you is no longer available.", Color.White);
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedPos = invitedE.Get<Position>()!;
        var invitedAppearance = invitedE.Get<PlayerAppearance>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            chatSender.Message(entityId, "You need to be close to the player to accept the trade.", Color.White);
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            chatSender.Message(entityId, "Who invited you is in the shop.", Color.White);
            return;
        }

        trade.Partner = invitedId.Value;
        var invitedTrade = invitedE.Get<TradeState>()!;
        invitedTrade.Partner = entityId;
        chatSender.Message(entityId, "You have accepted " + invitedAppearance.Name + "'s trade request.", Color.White);
        chatSender.Message(invitedId.Value, appearance.Name + " has accepted your trade request.", Color.White);

        trade.Request = string.Empty;
        trade.Offer = new TradeSlot[MaxInventory];
        invitedTrade.Offer = new TradeSlot[MaxInventory];

        tradeSender.Trade(entityId, true);
        tradeSender.Trade(invitedId.Value, true);
    }

    internal void Decline(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>()!;

        var invitedId = world.FindPlayer(trade.Request);
        if (invitedId != null) chatSender.Message(invitedId.Value, appearance.Name + " decline the trade.", Color.White);
        trade.Request = string.Empty;
    }

    public void Leave(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var trade = e.Get<TradeState>();
        if (trade == null || trade.Partner == null) return;

        var partnerE = world.Entities.Get(trade.Partner.Value)!;
        var partnerTrade = partnerE.Get<TradeState>()!;

        partnerTrade.Partner = null;
        tradeSender.Trade(trade.Partner.Value, false);
        trade.Partner = null;
        tradeSender.Trade(entityId, false);
    }

    internal void Offer(EntityId entityId, short slot, short inventorySlot, short amount)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;
        var trade = e.Get<TradeState>()!;

        amount = Math.Min(amount, inv.Slots[inventorySlot].Amount);

        if (inventorySlot != 0)
        {
            for (byte i = 0; i < MaxInventory; i++)
                if (trade.Offer[i].SlotNum == inventorySlot)
                    return;

            trade.Offer[slot].SlotNum = inventorySlot;
            trade.Offer[slot].Amount = amount;
        }
        else
            trade.Offer[slot] = new TradeSlot();

        tradeSender.TradeOffer(entityId);
        if (trade.Partner.HasValue) tradeSender.TradeOffer(trade.Partner.Value, false);
    }

    internal void OfferState(EntityId entityId, TradeStatus state)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;
        var trade = e.Get<TradeState>()!;

        var invitedId = trade.Partner;
        if (!invitedId.HasValue) return;

        var appearance = e.Get<PlayerAppearance>()!;
        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedInv = invitedE.Get<InventoryState>()!;
        var invitedAppearance = invitedE.Get<PlayerAppearance>()!;
        var invitedTrade = invitedE.Get<TradeState>()!;

        switch (state)
        {
            case TradeStatus.Accepted:
                if (trade.Offer.Count(x => x.SlotNum != 0) > invitedInv.TotalFree)
                {
                    chatSender.Message(invitedId.Value,
                        invitedAppearance.Name + " don't have enough space in their inventory to do this trade.", Color.Red);
                    break;
                }

                if (invitedTrade.Offer.Count(x => x.SlotNum != 0) > inv.TotalFree)
                {
                    chatSender.Message(invitedId.Value, "You don't have enough space in your inventory to do this trade.",
                        Color.Red);
                    break;
                }

                chatSender.Message(invitedId.Value, "The offer was accepted.", Color.Green);

                ItemSlot[] yourInventory = (ItemSlot[])inv.Slots.Clone(),
                    theirInventory = (ItemSlot[])invitedInv.Slots.Clone();

                var to = entityId;
                for (byte j = 0; j < 2; j++, to = to == entityId ? invitedId.Value : entityId)
                    for (byte i = 0; i < MaxInventory; i++)
                    {
                        var toInv = to == entityId ? inv : invitedInv;
                        var toTrade = to == entityId ? trade : invitedTrade;
                        inventorySystem.TakeItem(to, toInv.Slots[toTrade.Offer[i].SlotNum], toTrade.Offer[i].Amount);
                    }

                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (trade.Offer[i].SlotNum > 0)
                        inventorySystem.GiveItem(invitedId.Value, _catalog.Items.Get(yourInventory[trade.Offer[i].SlotNum].ItemId),
                            trade.Offer[i].Amount);
                    if (invitedTrade.Offer[i].SlotNum > 0)
                        inventorySystem.GiveItem(entityId, _catalog.Items.Get(theirInventory[invitedTrade.Offer[i].SlotNum].ItemId),
                            invitedTrade.Offer[i].Amount);
                }

                playerSender.PlayerInventory(entityId);
                playerSender.PlayerInventory(invitedId.Value);

                trade.Offer = new TradeSlot[MaxInventory];
                invitedTrade.Offer = new TradeSlot[MaxInventory];
                tradeSender.TradeOffer(invitedId.Value);
                tradeSender.TradeOffer(invitedId.Value, false);
                break;

            case TradeStatus.Declined:
                chatSender.Message(invitedId.Value, "The offer was declined.", Color.Red);
                break;

            case TradeStatus.Waiting:
                chatSender.Message(invitedId.Value, appearance.Name + " send you a offer.", Color.White);
                break;
        }

        tradeSender.TradeState(invitedId.Value, state);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case PlayerStartedMovingEvent e:
                    {
                        var playerId = world.FindPlayerByValue(e.PlayerId);
                        if (playerId != null) Leave(playerId.Value);
                        break;
                    }
                case PlayerWarpedEvent e:
                    {
                        var playerId = world.FindPlayerByValue(e.PlayerId);
                        if (playerId != null) Leave(playerId.Value);
                        break;
                    }
                case PlayerDisconnectedEvent e:
                    {
                        var playerId = world.FindPlayerByValue(e.PlayerId);
                        if (playerId != null) Leave(playerId.Value);
                        break;
                    }
            }
        }
    }
}
