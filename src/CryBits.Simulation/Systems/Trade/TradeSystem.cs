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
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
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

        var invitedE = world.Entities.Get(invitedId.Value);
        if (invitedE == null) return;
        var invitedTrade = invitedE.Get<TradeState>();
        var invitedPos = invitedE.Get<Position>()!;
        var invitedShop = invitedE.Get<ShopState>();

        if (invitedTrade?.Partner != null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player is already part of a trade.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedTrade?.PendingInviterId.HasValue == true)
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

        var trade = world.AddOrGet<TradeState>(invitedId.Value);
        if (trade == null) return;
        trade.PendingInviterId = entityId;
        world.MarkDirty<TradeState>(invitedId.Value);
    }

    private void Accept(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>();
        var shop = e.Get<ShopState>();

        var invitedId = trade?.PendingInviterId;

        if (trade?.Partner != null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You are already part of a trade.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedId == null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "Who invited you is no longer available.", ColorArgb = ChatColors.White });
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value);
        if (invitedE == null) return;
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

        var myTrade = world.AddOrGet<TradeState>(entityId);
        if (myTrade == null) return;
        var invitedTrade = world.AddOrGet<TradeState>(invitedId.Value);
        if (invitedTrade == null) return;

        myTrade.Partner = invitedId.Value;
        world.MarkDirty<TradeState>(entityId);
        invitedTrade.Partner = entityId;
        world.MarkDirty<TradeState>(invitedId.Value);
        tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You have accepted " + invitedAppearance.Name + "'s trade request.", ColorArgb = ChatColors.White });
        tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = appearance.Name + " has accepted your trade request.", ColorArgb = ChatColors.White });

        myTrade.PendingInviterId = null;
        myTrade.Offer = new TradeSlot[MaxInventory];
        invitedTrade.Offer = new TradeSlot[MaxInventory];

        world.MarkDirty<TradeState>(entityId);
        world.MarkDirty<TradeState>(invitedId.Value);
    }

    private void Decline(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>();
        if (trade == null) return;

        var invitedId = trade.PendingInviterId;
        if (invitedId != null) tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = appearance.Name + " decline the trade.", ColorArgb = ChatColors.White });
        world.Remove<TradeState>(entityId);
    }

    private void Leave(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var trade = e.Get<TradeState>();
        if (trade == null || trade.Partner == null) return;

        var partnerTrade = world.Get<TradeState>(trade.Partner.Value);
        if (partnerTrade != null)
        {
            partnerTrade.Partner = null;
            world.MarkDirty<TradeState>(trade.Partner.Value);
            world.Remove<TradeState>(trade.Partner.Value);
        }
        trade.Partner = null;
        world.MarkDirty<TradeState>(entityId);
        world.Remove<TradeState>(entityId);
    }

    private void Offer(World world, Tick tick, EntityId entityId, short slot, short inventorySlot, short amount)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var inv = e.Get<InventoryState>()!;
        var trade = e.Get<TradeState>();
        if (trade?.Offer == null) return;
        var offer = trade.Offer;

        amount = Math.Min(amount, inv.Slots[inventorySlot].Amount);

        if (inventorySlot != 0)
        {
            for (byte i = 0; i < MaxInventory; i++)
                if (offer[i].SlotNum == inventorySlot)
                    return;

            offer[slot].SlotNum = inventorySlot;
            offer[slot].Amount = amount;
        }
        else
            offer[slot] = new TradeSlot();

        world.MarkDirty<TradeState>(entityId);
        if (trade.Partner.HasValue) world.MarkDirty<TradeState>(trade.Partner.Value);
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
                    tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = invitedAppearance.Name + " don't have enough space in their inventory to do this trade.", ColorArgb = ChatColors.Red });
                    break;
                }

                if (invitedTrade.Offer is null || invitedTrade.Offer.Count(x => x.SlotNum != 0) > invFree)
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
                        if (toTrade.Offer is not null && toTrade.Offer[i].SlotNum > 0)
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

                world.MarkDirty<InventoryState>(entityId);
                world.MarkDirty<InventoryState>(invitedId.Value);

                trade.Offer = new TradeSlot[MaxInventory];
                invitedTrade.Offer = new TradeSlot[MaxInventory];
                world.MarkDirty<TradeState>(invitedId.Value);
                world.MarkDirty<TradeState>(entityId);
                break;

            case TradeStatus.Declined:
                tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = "The offer was declined.", ColorArgb = ChatColors.Red });
                break;

            case TradeStatus.Waiting:
                tick.Events.Emit(new ChatMessageEvent { RecipientId = invitedId.Value, Text = appearance.Name + " send you a offer.", ColorArgb = ChatColors.White });
                break;
        }
    }

}
