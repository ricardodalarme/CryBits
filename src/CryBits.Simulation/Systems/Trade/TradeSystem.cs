using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using static CryBits.Definitions.Globals;

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
                case TradeLeaveIntent l: Leave(world, l.SourceEntityId); break;
                case TradeOfferIntent o: Offer(world, o.SourceEntityId, o.OfferSlot, o.InventorySlot, o.Amount); break;
                case TradeOfferStateIntent s: OfferState(world, tick, s.SourceEntityId, s.State); break;
            }
        }

        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case PlayerStartedMovingEvent e:
                    {
                        if (world.Has<PlayerTag>(e.PlayerId)) Leave(world, e.PlayerId);
                        break;
                    }
                case PlayerWarpedEvent e:
                    {
                        if (world.Has<PlayerTag>(e.PlayerId)) Leave(world, e.PlayerId);
                        break;
                    }
                case PlayerDisconnectedEvent e:
                    {
                        if (world.Has<PlayerTag>(e.PlayerId)) Leave(world, e.PlayerId);
                        break;
                    }
            }
        }
    }

    private void Invite(World world, Tick tick, EntityId entityId, string targetName)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;

        var pos = e.Get<Position>()!;
        var shop = e.Get<ShopState>();

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The player isn't connected.", ChatColors.White));
            return;
        }

        if (invitedId.Value == entityId)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You can't be invited.", ChatColors.White));
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value);
        if (invitedE == null) return;
        var invitedTrade = invitedE.Get<TradeState>();
        var invitedPos = invitedE.Get<Position>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (invitedTrade?.Partner != null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The player is already part of a trade.", ChatColors.White));
            return;
        }

        if (invitedTrade?.PendingInviterId.HasValue == true)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The player is analyzing an invitation of another trade.", ChatColors.White));
            return;
        }

        if (shop?.ShopId != null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You can't start a trade while in the shop.", ChatColors.White));
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The player is in the shop.", ChatColors.White));
            return;
        }

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You need to be close to the player to start trade.", ChatColors.White));
            return;
        }

        if (!world.Has<TradeState>(invitedId.Value))
            world.Set(invitedId.Value, new TradeState(PendingInviterId: entityId));
        else
            world.Update<TradeState>(invitedId.Value, t => t with { PendingInviterId = entityId });
    }

    private void Accept(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>();

        var invitedId = trade?.PendingInviterId;

        if (trade?.Partner != null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You are already part of a trade.", ChatColors.White));
            return;
        }

        if (invitedId == null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "Who invited you is no longer available.", ChatColors.White));
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value);
        if (invitedE == null) return;
        var invitedPos = invitedE.Get<Position>()!;
        var invitedAppearance = invitedE.Get<PlayerAppearance>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (Math.Abs(pos.X - invitedPos.X) + Math.Abs(pos.Y - invitedPos.Y) != 1)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You need to be close to the player to accept the trade.", ChatColors.White));
            return;
        }

        if (invitedShop?.ShopId != null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "Who invited you is in the shop.", ChatColors.White));
            return;
        }

        world.Set(entityId, new TradeState(Partner: invitedId.Value, Offer: new TradeSlot[MaxInventory]));
        world.Set(invitedId.Value, new TradeState(Partner: entityId, Offer: new TradeSlot[MaxInventory]));
        tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You have accepted " + invitedAppearance.Name + "'s trade request.", ChatColors.White));
        tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, invitedId.Value, appearance.Name + " has accepted your trade request.", ChatColors.White));
    }

    private void Decline(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>();
        if (trade == null) return;

        var invitedId = trade.PendingInviterId;
        if (invitedId != null) tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, invitedId.Value, appearance.Name + " decline the trade.", ChatColors.White));
        world.Remove<TradeState>(entityId);
    }

    private void Leave(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var trade = e.Get<TradeState>();
        if (trade == null || trade.Partner == null) return;

        world.Remove<TradeState>(trade.Partner.Value);
        world.Remove<TradeState>(entityId);
    }

    private void Offer(World world, EntityId entityId, short slot, short inventorySlot, short amount)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var inv = e.Get<InventoryState>()!;
        var trade = e.Get<TradeState>();
        if (trade?.Offer == null) return;
        var offer = trade.Offer;

        amount = Math.Min(amount, inv.Slots[inventorySlot].Amount);

        var newOffer = (TradeSlot[])offer.Clone();
        if (inventorySlot != 0)
        {
            for (byte i = 0; i < MaxInventory; i++)
                if (newOffer[i].SlotNum == inventorySlot)
                    return;

            newOffer[slot] = new TradeSlot { SlotNum = inventorySlot, Amount = amount };
        }
        else
            newOffer[slot] = new TradeSlot();

        world.Set(entityId, new TradeState(Partner: trade.Partner, PendingInviterId: trade.PendingInviterId, Offer: newOffer, TheirOffer: trade.TheirOffer));
    }

    private void OfferState(World world, Tick tick, EntityId entityId, TradeStatus state)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var inv = e.Get<InventoryState>()!;
        var trade = e.Get<TradeState>();
        if (trade == null) return;

        var invitedId = trade.Partner;
        if (!invitedId.HasValue) return;

        var appearance = e.Get<PlayerAppearance>()!;
        var invitedE = world.Entities.Get(invitedId.Value);
        if (invitedE == null) return;
        var invitedInv = invitedE.Get<InventoryState>()!;
        var invitedAppearance = invitedE.Get<PlayerAppearance>()!;
        var invitedTrade = invitedE.Get<TradeState>();
        if (invitedTrade == null) return;

        switch (state)
        {
            case TradeStatus.Accepted:
                var invFree = inv.CountFreeSlots();
                var invitedFree = invitedInv.CountFreeSlots();

                if (trade.Offer is null || trade.Offer.Count(x => x.SlotNum != 0) > invitedFree)
                {
                    tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, invitedId.Value, invitedAppearance.Name + " don't have enough space in their inventory to do this trade.", ChatColors.Red));
                    break;
                }

                if (invitedTrade.Offer is null || invitedTrade.Offer.Count(x => x.SlotNum != 0) > invFree)
                {
                    tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, invitedId.Value, "You don't have enough space in your inventory to do this trade.", ChatColors.Red));
                    break;
                }

                tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, invitedId.Value, "The offer was accepted.", ChatColors.Green));

                ItemSlot[] yourInventory = (ItemSlot[])inv.Slots.Clone(),
                    theirInventory = (ItemSlot[])invitedInv.Slots.Clone();

                for (byte i = 0; i < MaxInventory; i++)
                    for (byte j = 0; j < 2; j++)
                    {
                        var to = j == 0 ? entityId : invitedId.Value;
                        var toTrade = j == 0 ? trade : invitedTrade;
                        if (toTrade.Offer is not null && toTrade.Offer[i].SlotNum > 0)
                            tick.Events.Emit(new ItemTakenEvent(tick.TickNumber, to, (byte)toTrade.Offer[i].SlotNum, toTrade.Offer[i].Amount));
                    }

                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (trade.Offer[i].SlotNum > 0)
                        tick.Events.Emit(new ItemGivenEvent(tick.TickNumber, invitedId.Value, yourInventory[trade.Offer[i].SlotNum].ItemId, trade.Offer[i].Amount));
                    if (invitedTrade.Offer[i].SlotNum > 0)
                        tick.Events.Emit(new ItemGivenEvent(tick.TickNumber, entityId, theirInventory[invitedTrade.Offer[i].SlotNum].ItemId, invitedTrade.Offer[i].Amount));
                }

                world.Set(entityId, new TradeState(Partner: trade.Partner, PendingInviterId: trade.PendingInviterId, Offer: new TradeSlot[MaxInventory], TheirOffer: trade.TheirOffer));
                world.Set(invitedId.Value, new TradeState(Partner: invitedTrade.Partner, PendingInviterId: invitedTrade.PendingInviterId, Offer: new TradeSlot[MaxInventory], TheirOffer: invitedTrade.TheirOffer));
                break;

            case TradeStatus.Declined:
                tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, invitedId.Value, "The offer was declined.", ChatColors.Red));
                break;

            case TradeStatus.Waiting:
                tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, invitedId.Value, appearance.Name + " send you a offer.", ChatColors.White));
                break;
        }
    }

}
