using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Trade;

public sealed class TradeSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case TradeInviteIntent i: Invite(world, tick, i.SourceEntityId, i.PlayerName); break;
                case TradeAcceptIntent a: Accept(world, tick, a.SourceEntityId); break;
                case TradeDeclineIntent d: Decline(world, tick, d.SourceEntityId); break;
                case TradeLeaveIntent l: Leave(world, tick, l.SourceEntityId); break;
                case TradeOfferIntent o: Offer(world, tick, o.SourceEntityId, o.OfferSlot, o.InventorySlot, o.Amount); break;
                case TradeOfferStateIntent s: OfferState(world, tick, s.SourceEntityId, s.State); break;
            }
        }

        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case PlayerStartedMovingEvent e:
                    {
                        var playerId = world.FindPlayer(e.PlayerId);
                        if (playerId != null) Leave(world, tick, playerId.Value);
                        break;
                    }
                case PlayerWarpedEvent e:
                    {
                        var playerId = world.FindPlayer(e.PlayerId);
                        if (playerId != null) Leave(world, tick, playerId.Value);
                        break;
                    }
                case PlayerDisconnectedEvent e:
                    {
                        var playerId = world.FindPlayer(e.PlayerId);
                        if (playerId != null) Leave(world, tick, playerId.Value);
                        break;
                    }
            }
        }
    }

    private void Invite(World world, Tick tick, EntityId entityId, string targetName)
    {
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>()!;
        var shop = e.Get<ShopState>();

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player isn't connected.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedId.Value == entityId)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You can't be invited.", ColorArgb = ChatColors.White });
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedTrade = invitedE.Get<TradeState>()!;
        var invitedPos = invitedE.Get<Position>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (invitedTrade.Partner != null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player is already part of a trade.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedTrade.PendingInviterId.HasValue)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player is analyzing an invitation of another trade.", ColorArgb = ChatColors.White });
            return;
        }

        if (shop?.ShopId != null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You can't start a trade while in the shop.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player is in the shop.", ColorArgb = ChatColors.White });
            return;
        }

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You need to be close to the player to start trade.", ColorArgb = ChatColors.White });
            return;
        }

        invitedTrade.PendingInviterId = entityId;
        world.Dirty.Mark<TradeState>(invitedId.Value);
    }

    private void Accept(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>()!;
        var shop = e.Get<ShopState>();

        var invitedId = trade.PendingInviterId;

        if (trade.Partner != null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You are already part of a trade.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedId == null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "Who invited you is no longer available.", ColorArgb = ChatColors.White });
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedPos = invitedE.Get<Position>()!;
        var invitedAppearance = invitedE.Get<PlayerAppearance>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You need to be close to the player to accept the trade.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "Who invited you is in the shop.", ColorArgb = ChatColors.White });
            return;
        }

        trade.Partner = invitedId.Value;
        world.Dirty.Mark<TradeState>(entityId);
        var invitedTrade = invitedE.Get<TradeState>()!;
        invitedTrade.Partner = entityId;
        world.Dirty.Mark<TradeState>(invitedId.Value);
        tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You have accepted " + invitedAppearance.Name + "'s trade request.", ColorArgb = ChatColors.White });
        tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = appearance.Name + " has accepted your trade request.", ColorArgb = ChatColors.White });

        trade.PendingInviterId = null;
        trade.Offer = new TradeSlot[MaxInventory];
        invitedTrade.Offer = new TradeSlot[MaxInventory];

        world.Dirty.Mark<TradeState>(entityId);
        world.Dirty.Mark<TradeState>(invitedId.Value);
    }

    private void Decline(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>()!;

        var invitedId = trade.PendingInviterId;
        if (invitedId != null) tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = appearance.Name + " decline the trade.", ColorArgb = ChatColors.White });
        trade.PendingInviterId = null;
        world.Dirty.Mark<TradeState>(entityId);
    }

    private void Leave(World world, Tick tick, EntityId entityId)
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

    private void Offer(World world, Tick tick, EntityId entityId, short slot, short inventorySlot, short amount)
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

    private void OfferState(World world, Tick tick, EntityId entityId, TradeStatus state)
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
                var invFree = CountFreeSlots(inv);
                var invitedFree = CountFreeSlots(invitedInv);

                if (trade.Offer.Count(x => x.SlotNum != 0) > invitedFree)
                {
                    tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = invitedAppearance.Name + " don't have enough space in their inventory to do this trade.", ColorArgb = ChatColors.Red });
                    break;
                }

                if (invitedTrade.Offer.Count(x => x.SlotNum != 0) > invFree)
                {
                    tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = "You don't have enough space in your inventory to do this trade.", ColorArgb = ChatColors.Red });
                    break;
                }

                tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = "The offer was accepted.", ColorArgb = ChatColors.Green });

                ItemSlot[] yourInventory = (ItemSlot[])inv.Slots.Clone(),
                    theirInventory = (ItemSlot[])invitedInv.Slots.Clone();

                for (byte i = 0; i < MaxInventory; i++)
                    for (byte j = 0; j < 2; j++)
                    {
                        var to = j == 0 ? entityId : invitedId.Value;
                        var toTrade = j == 0 ? trade : invitedTrade;
                        if (toTrade.Offer[i].SlotNum > 0)
                            tick.Events.Emit(new ItemTakenEvent
                            {
                                EntityId = to,
                                SlotIndex = (byte)toTrade.Offer[i].SlotNum,
                                Amount = toTrade.Offer[i].Amount
                            });
                    }

                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (trade.Offer[i].SlotNum > 0)
                        tick.Events.Emit(new ItemGivenEvent
                        {
                            EntityId = invitedId.Value,
                            ItemId = yourInventory[trade.Offer[i].SlotNum].ItemId,
                            Amount = trade.Offer[i].Amount
                        });
                    if (invitedTrade.Offer[i].SlotNum > 0)
                        tick.Events.Emit(new ItemGivenEvent
                        {
                            EntityId = entityId,
                            ItemId = theirInventory[invitedTrade.Offer[i].SlotNum].ItemId,
                            Amount = invitedTrade.Offer[i].Amount
                        });
                }

                world.Dirty.Mark<InventoryState>(entityId);
                world.Dirty.Mark<InventoryState>(invitedId.Value);

                trade.Offer = new TradeSlot[MaxInventory];
                invitedTrade.Offer = new TradeSlot[MaxInventory];
                world.Dirty.Mark<TradeState>(invitedId.Value);
                world.Dirty.Mark<TradeState>(entityId);
                break;

            case TradeStatus.Declined:
                tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = "The offer was declined.", ColorArgb = ChatColors.Red });
                break;

            case TradeStatus.Waiting:
                tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = appearance.Name + " send you a offer.", ColorArgb = ChatColors.White });
                break;
        }
    }

    private static byte CountFreeSlots(InventoryState inv)
    {
        byte count = 0;
        for (var i = 0; i < inv.Slots.Length; i++)
            if (inv.Slots[i].ItemId == Guid.Empty) count++;
        return count;
    }
}
