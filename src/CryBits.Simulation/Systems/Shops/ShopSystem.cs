using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
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
                        if (world.Has<PlayerTag>(e.PlayerId)) Leave(world, e.PlayerId);
                        break;
                    }
                case PlayerWarpedEvent e:
                    {
                        if (world.Has<PlayerTag>(e.PlayerId)) Leave(world, e.PlayerId);
                        break;
                    }
                case NpcAttackedEvent e:
                    {
                        if (!world.Has<PlayerTag>(e.AttackerId) || !world.Has<NpcTag>(e.NpcInstanceId)) break;
                        var npcE = world.Entities.Get(e.NpcInstanceId);
                        if (npcE == null) break;
                        var npcState = npcE.Get<NpcState>()!;
                        var npcData = catalog.Npcs.Get(npcState.NpcDefId);
                        if (npcData is null) break;
                        if (npcData.Behaviour == Behaviour.ShopKeeper)
                        {
                            var shop = catalog.Shops.Get(npcData.ShopId);
                            if (shop != null) Open(world, e.AttackerId, shop);
                        }
                        break;
                    }
            }
        }
    }

    private void Open(World world, EntityId entityId, Shop shop)
    {
        world.Set(entityId, new ShopState(ShopId: shop.Id));
    }

    private void Leave(World world, EntityId entityId)
    {
        world.Remove<ShopState>(entityId);
    }

    private void Buy(World world, Tick tick, EntityId entityId, short shopSoldIndex)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var shopState = e.Get<ShopState>();
        if (shopState?.ShopId == null) return;
        var inv = e.Get<InventoryState>()!;
        var shop = catalog.Shops.Get(shopState.ShopId.Value);
        if (shop is null) return;
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
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You don't have enough money to buy the item.", ChatColors.Red));
            return;
        }

        if (inv.CountFreeSlots() == 0 && inv.Slots[currencySlot.Value].Amount > shopSold.Price)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You don't have space in your bag.", ChatColors.Red));
            return;
        }

        var soldItem = catalog.Items.Get(shopSold.ItemId);
        var soldItemName = soldItem?.Name ?? "Unknown";
        tick.Events.Emit(new ItemTakenEvent(tick.TickNumber, entityId, (byte)currencySlot.Value, shopSold.Price));
        tick.Events.Emit(new ItemGivenEvent(tick.TickNumber, entityId, shopSold.ItemId, shopSold.Amount));
        tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You bought " + shopSold.Price + "x " + soldItemName + ".", ChatColors.Green));
    }

    private void Sell(World world, Tick tick, EntityId entityId, byte inventorySlotIndex, short amount)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var shopState = e.Get<ShopState>();
        if (shopState?.ShopId == null) return;
        var inv = e.Get<InventoryState>()!;

        var shop = catalog.Shops.Get(shopState.ShopId.Value);
        if (shop is null) return;

        amount = Math.Min(amount, inv.Slots[inventorySlotIndex].Amount);
        var buy = shop.FindBought(inv.Slots[inventorySlotIndex].ItemId);

        if (buy == null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The store doesn't buy this item", ChatColors.Red));
            return;
        }

        if (inv.CountFreeSlots() == 0 && inv.Slots[inventorySlotIndex].Amount > amount)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You don't have space in your bag.", ChatColors.Red));
            return;
        }

        var soldItem = catalog.Items.Get(inv.Slots[inventorySlotIndex].ItemId);
        var soldItemName = soldItem?.Name ?? "Unknown";

        tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You sold " + amount + "x " + soldItemName + " for " + buy.Price * amount + ".", ChatColors.Green));
        tick.Events.Emit(new ItemTakenEvent(tick.TickNumber, entityId, inventorySlotIndex, amount));
        tick.Events.Emit(new ItemGivenEvent(tick.TickNumber, entityId, shop.CurrencyId, (short)(buy.Price * amount)));
    }
}
