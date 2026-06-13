using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Slots;
using System.Linq;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Inventory;
using System;
using System.Drawing;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Trade;

internal sealed class TradeSystem(
    InventorySystem inventorySystem,
    DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static TradeSystem Instance { get; } = new(
        InventorySystem.Instance,
        DefinitionCatalog.Instance);

    internal void Invite(World world, EntityId entityId, string targetName)
    {
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>()!;
        var shop = e.Get<ShopState>();

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The player isn't connected.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (invitedId.Value == entityId)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You can't be invited.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedTrade = invitedE.Get<TradeState>()!;
        var invitedPos = invitedE.Get<Position>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (invitedTrade.Partner != null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The player is already part of a trade.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (!string.IsNullOrEmpty(invitedTrade.Request))
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The player is analyzing an invitation of another trade.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (shop?.ShopId != null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You can't start a trade while in the shop.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The player is in the shop.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You need to be close to the player to start trade.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        invitedTrade.Request = appearance.Name;
        world.Dirty.Mark<TradeState>(invitedId.Value);
    }

    internal void Accept(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>()!;
        var shop = e.Get<ShopState>();

        var invitedId = world.FindPlayer(trade.Request);

        if (trade.Partner != null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You are already part of a trade.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (invitedId == null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "Who invited you is no longer available.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedPos = invitedE.Get<Position>()!;
        var invitedAppearance = invitedE.Get<PlayerAppearance>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You need to be close to the player to accept the trade.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "Who invited you is in the shop.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        trade.Partner = invitedId.Value;
        world.Dirty.Mark<TradeState>(entityId);
        var invitedTrade = invitedE.Get<TradeState>()!;
        invitedTrade.Partner = entityId;
        world.Dirty.Mark<TradeState>(invitedId.Value);
        world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You have accepted " + invitedAppearance.Name + "'s trade request.", ColorArgb = Color.White.ToArgb() });
        world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value.Value, Text = appearance.Name + " has accepted your trade request.", ColorArgb = Color.White.ToArgb() });

        trade.Request = string.Empty;
        trade.Offer = new TradeSlot[MaxInventory];
        invitedTrade.Offer = new TradeSlot[MaxInventory];

        world.Dirty.Mark<TradeState>(entityId);
        world.Dirty.Mark<TradeState>(invitedId.Value);
    }

    internal void Decline(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>()!;

        var invitedId = world.FindPlayer(trade.Request);
        if (invitedId != null) world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value.Value, Text = appearance.Name + " decline the trade.", ColorArgb = Color.White.ToArgb() });
        trade.Request = string.Empty;
        world.Dirty.Mark<TradeState>(entityId);
    }

    public void Leave(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var trade = e.Get<TradeState>();
        if (trade == null || trade.Partner == null) return;

        var partnerE = world.Entities.Get(trade.Partner.Value)!;
        var partnerTrade = partnerE.Get<TradeState>()!;

        partnerTrade.Partner = null;
        world.Dirty.Mark<TradeState>(trade.Partner.Value);
        trade.Partner = null;
        world.Dirty.Mark<TradeState>(entityId);
    }

    internal void Offer(World world, EntityId entityId, short slot, short inventorySlot, short amount)
    {
        var e = world.Entities.Get(entityId)!;
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

        world.Dirty.Mark<TradeState>(entityId);
        if (trade.Partner.HasValue) world.Dirty.Mark<TradeState>(trade.Partner.Value);
    }

    internal void OfferState(World world, EntityId entityId, TradeStatus state)
    {
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
                    world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value.Value, Text = invitedAppearance.Name + " don't have enough space in their inventory to do this trade.", ColorArgb = Color.Red.ToArgb() });
                    break;
                }

                if (invitedTrade.Offer.Count(x => x.SlotNum != 0) > inv.TotalFree)
                {
                    world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value.Value, Text = "You don't have enough space in your inventory to do this trade.", ColorArgb = Color.Red.ToArgb() });
                    break;
                }

                world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value.Value, Text = "The offer was accepted.", ColorArgb = Color.Green.ToArgb() });

                ItemSlot[] yourInventory = (ItemSlot[])inv.Slots.Clone(),
                    theirInventory = (ItemSlot[])invitedInv.Slots.Clone();

                var to = entityId;
                for (byte j = 0; j < 2; j++, to = to == entityId ? invitedId.Value : entityId)
                    for (byte i = 0; i < MaxInventory; i++)
                    {
                        var toInv = to == entityId ? inv : invitedInv;
                        var toTrade = to == entityId ? trade : invitedTrade;
                        inventorySystem.TakeItem(world, to, toInv.Slots[toTrade.Offer[i].SlotNum], toTrade.Offer[i].Amount);
                    }

                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (trade.Offer[i].SlotNum > 0)
                        inventorySystem.GiveItem(world, invitedId.Value, _catalog.Items.Get(yourInventory[trade.Offer[i].SlotNum].ItemId),
                            trade.Offer[i].Amount);
                    if (invitedTrade.Offer[i].SlotNum > 0)
                        inventorySystem.GiveItem(world, entityId, _catalog.Items.Get(theirInventory[invitedTrade.Offer[i].SlotNum].ItemId),
                            invitedTrade.Offer[i].Amount);
                }

                world.Dirty.Mark<InventoryState>(entityId);
                world.Dirty.Mark<InventoryState>(invitedId.Value);

                trade.Offer = new TradeSlot[MaxInventory];
                invitedTrade.Offer = new TradeSlot[MaxInventory];
                world.Dirty.Mark<TradeState>(invitedId.Value);
                world.Dirty.Mark<TradeState>(entityId);
                break;

            case TradeStatus.Declined:
                world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value.Value, Text = "The offer was declined.", ColorArgb = Color.Red.ToArgb() });
                break;

            case TradeStatus.Waiting:
                world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value.Value, Text = appearance.Name + " send you a offer.", ColorArgb = Color.White.ToArgb() });
                break;
        }
    }

    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case TradeInviteIntent i: Invite(world, i.SourceEntityId, i.PlayerName); break;
                case TradeAcceptIntent a: Accept(world, a.SourceEntityId); break;
                case TradeDeclineIntent d: Decline(world, d.SourceEntityId); break;
                case TradeLeaveIntent l: Leave(world, l.SourceEntityId); break;
                case TradeOfferIntent o: Offer(world, o.SourceEntityId, o.OfferSlot, o.InventorySlot, o.Amount); break;
                case TradeOfferStateIntent s: OfferState(world, s.SourceEntityId, s.State); break;
            }
        }

        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case PlayerStartedMovingEvent e:
                    {
                        var playerId = world.FindPlayerByValue(e.PlayerId);
                        if (playerId != null) Leave(world, playerId.Value);
                        break;
                    }
                case PlayerWarpedEvent e:
                    {
                        var playerId = world.FindPlayerByValue(e.PlayerId);
                        if (playerId != null) Leave(world, playerId.Value);
                        break;
                    }
                case PlayerDisconnectedEvent e:
                    {
                        var playerId = world.FindPlayerByValue(e.PlayerId);
                        if (playerId != null) Leave(world, playerId.Value);
                        break;
                    }
            }
        }
    }
}
