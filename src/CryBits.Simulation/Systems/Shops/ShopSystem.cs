using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using System;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Shops;

public sealed class ShopSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case ShopBuyIntent b: Buy(world, tick, b.SourceEntityId, b.Slot); break;
                case ShopSellIntent s: Sell(world, tick, s.SourceEntityId, s.InventorySlot, s.Amount); break;
                case ShopCloseIntent c: Leave(world, c.SourceEntityId); break;
            }
        }

        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case PlayerStartedMovingEvent e:
                    {
                        var playerId = world.FindPlayer(e.PlayerId);
                        if (playerId != null) Leave(world, playerId.Value);
                        break;
                    }
                case PlayerWarpedEvent e:
                    {
                        var playerId = world.FindPlayer(e.PlayerId);
                        if (playerId != null) Leave(world, playerId.Value);
                        break;
                    }
                case NpcAttackedEvent e:
                    {
                        var attackerId = world.FindPlayer(e.AttackerId);
                        var npcId = world.FindNpcInstance(e.NpcInstanceId);
                        if (attackerId == null || npcId == null) break;
                        var npcE = world.Entities.Get(npcId.Value)!;
                        var npcState = npcE.Get<NpcState>()!;
                        var npcData = catalog.Npcs.Get(npcState.NpcDefId);
                        if (npcData.Behaviour == Behaviour.ShopKeeper)
                        {
                            var shop = catalog.Shops.Get(npcData.ShopId);
                            if (shop != null) Open(world, attackerId.Value, shop);
                        }
                        break;
                    }
            }
        }
    }

    private void Open(World world, EntityId entityId, Shop shop)
    {
        var e = world.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;

        shopState.ShopId = shop.Id;
        world.Dirty.Mark<ShopState>(entityId);
    }

    private void Leave(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;

        if (shopState.ShopId == null) return;

        shopState.ShopId = null;
        world.Dirty.Mark<ShopState>(entityId);
    }

    private static byte CountFreeSlots(InventoryState inv)
    {
        byte count = 0;
        for (var i = 0; i < inv.Slots.Length; i++)
            if (inv.Slots[i].ItemId == Guid.Empty) count++;
        return count;
    }

    private void Buy(World world, Tick tick, EntityId entityId, short shopSoldIndex)
    {
        var e = world.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;
        var inv = e.Get<InventoryState>()!;
        var shop = catalog.Shops.Get(shopState.ShopId!.Value);
        var shopSold = shop.Sold[shopSoldIndex];

        if (shop.CurrencyId == Guid.Empty) return;

        int? currencySlot = null;
        for (var i = 0; i < inv.Slots.Length; i++)
        {
            if (inv.Slots[i].ItemId == shop.CurrencyId)
            {
                currencySlot = i;
                break;
            }
        }

        if (currencySlot == null || inv.Slots[currencySlot.Value].Amount < shopSold.Price)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You don't have enough money to buy the item.", ColorArgb = ChatColors.Red });
            return;
        }

        if (CountFreeSlots(inv) == 0 && inv.Slots[currencySlot.Value].Amount > shopSold.Price)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You don't have space in your bag.", ColorArgb = ChatColors.Red });
            return;
        }

        var soldItem = catalog.Items.Get(shopSold.ItemId);
        var soldItemName = soldItem?.Name ?? "Unknown";
        tick.Events.Emit(new ItemTakenEvent { EntityId = entityId, SlotIndex = (byte)currencySlot.Value, Amount = shopSold.Price });
        tick.Events.Emit(new ItemGivenEvent { EntityId = entityId, ItemId = shopSold.ItemId, Amount = shopSold.Amount });
        tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You bought " + shopSold.Price + "x " + soldItemName + ".", ColorArgb = ChatColors.Green });
    }

    private void Sell(World world, Tick tick, EntityId entityId, byte inventorySlotIndex, short amount)
    {
        var e = world.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;
        var inv = e.Get<InventoryState>()!;

        var shop = catalog.Shops.Get(shopState.ShopId!.Value);

        amount = Math.Min(amount, inv.Slots[inventorySlotIndex].Amount);
        var buy = shop.FindBought(inv.Slots[inventorySlotIndex].ItemId);

        if (buy == null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The store doesn't buy this item", ColorArgb = ChatColors.Red });
            return;
        }

        if (CountFreeSlots(inv) == 0 && inv.Slots[inventorySlotIndex].Amount > amount)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You don't have space in your bag.", ColorArgb = ChatColors.Red });
            return;
        }

        var soldItem = catalog.Items.Get(inv.Slots[inventorySlotIndex].ItemId);
        var soldItemName = soldItem?.Name ?? "Unknown";

        tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You sold " + amount + "x " + soldItemName + " for " + buy.Price * amount + ".", ColorArgb = ChatColors.Green });
        tick.Events.Emit(new ItemTakenEvent { EntityId = entityId, SlotIndex = inventorySlotIndex, Amount = amount });
        tick.Events.Emit(new ItemGivenEvent { EntityId = entityId, ItemId = shop.CurrencyId, Amount = (short)(buy.Price * amount) });
    }
}
