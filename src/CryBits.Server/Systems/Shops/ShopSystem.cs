using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Inventory;
using CryBits.Server.World;
using System;
using System.Drawing;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Shops;

internal sealed class ShopSystem(
    InventorySystem inventorySystem,
    DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ShopSystem Instance { get; } = new(
        InventorySystem.Instance,
        DefinitionCatalog.Instance);

    public void Open(EntityId entityId, Shop shop)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;

        shopState.ShopId = shop.Id;
        GameWorld.Current.Dirty.Mark<ShopState>(entityId);
    }

    public void Leave(EntityId entityId)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;

        if (shopState.ShopId == null) return;

        shopState.ShopId = null;
        GameWorld.Current.Dirty.Mark<ShopState>(entityId);
    }

    internal void Buy(EntityId entityId, short shopSoldIndex)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;
        var inv = e.Get<InventoryState>()!;
        var catalog = DefinitionCatalog.Instance;

        var shop = _catalog.Shops.Get(shopState.ShopId!.Value);
        var shopSold = shop.Sold[shopSoldIndex];

        if (shop.CurrencyId == Guid.Empty) return;

        var inventorySlot = inv.Find(shop.CurrencyId);

        if (inventorySlot == null || inventorySlot.Amount < shopSold.Price)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You don't have enough money to buy the item.", ColorArgb = Color.Red.ToArgb() });
            return;
        }

        if (inv.TotalFree == 0 && inventorySlot.Amount > shopSold.Price)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You don't have space in your bag.", ColorArgb = Color.Red.ToArgb() });
            return;
        }

        var soldItem = _catalog.Items.Get(shopSold.ItemId);
        var soldItemName = soldItem?.Name ?? "Unknown";
        inventorySystem.TakeItem(entityId, inventorySlot, shopSold.Price);
        inventorySystem.GiveItem(entityId, soldItem, shopSold.Amount);
        GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You bought " + shopSold.Price + "x " + soldItemName + ".", ColorArgb = Color.Green.ToArgb() });
    }

    internal void Sell(EntityId entityId, byte inventorySlotIndex, short amount)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var shopState = e.Get<ShopState>()!;
        var inv = e.Get<InventoryState>()!;

        var shop = _catalog.Shops.Get(shopState.ShopId!.Value);

        amount = Math.Min(amount, inv.Slots[inventorySlotIndex].Amount);
        var buy = shop.FindBought(inv.Slots[inventorySlotIndex].ItemId);

        if (buy == null)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The store doesn't sell this item", ColorArgb = Color.Red.ToArgb() });
            return;
        }

        if (inv.TotalFree == 0 && inv.Slots[inventorySlotIndex].Amount > amount)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You don't have space in your bag.", ColorArgb = Color.Red.ToArgb() });
            return;
        }

        var soldItem = _catalog.Items.Get(inv.Slots[inventorySlotIndex].ItemId);
        var soldItemName = soldItem?.Name ?? "Unknown";
        var currencyItem = _catalog.Items.Get(shop.CurrencyId);
        GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You sold " + soldItemName + "x " + amount + " for .", ColorArgb = Color.Green.ToArgb() });
        inventorySystem.TakeItem(entityId, inv.Slots[inventorySlotIndex], amount);
        inventorySystem.GiveItem(entityId, currencyItem, (short)(buy.Price * amount));
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case ShopBuyIntent b: Buy(b.SourceEntityId, b.Slot); break;
                case ShopSellIntent s: Sell(s.SourceEntityId, s.InventorySlot, s.Amount); break;
                case ShopCloseIntent c: Leave(c.SourceEntityId); break;
            }
        }

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
                case NpcAttackedEvent e:
                    {
                        var attackerId = world.FindPlayerByValue(e.AttackerId);
                        var npcId = world.FindNpcInstance(e.NpcInstanceId);
                        if (attackerId == null || npcId == null) break;
                        var npcE = world.Entities.Get(npcId.Value)!;
                        var npcState = npcE.Get<NpcState>()!;
                        var npcData = _catalog.Npcs.Get(npcState.NpcDefId);
                        if (npcData.Behaviour == Behaviour.ShopKeeper)
                        {
                            var shop = _catalog.Shops.Get(npcData.ShopId);
                            if (shop != null) Open(attackerId.Value, shop);
                        }
                        break;
                    }
            }
        }
    }
}
