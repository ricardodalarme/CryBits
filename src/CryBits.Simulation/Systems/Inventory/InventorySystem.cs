using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using System;
using System.Drawing;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using CryBits.Simulation.Entities;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class InventorySystem(DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;

    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case CollectItemIntent collect:
                    CollectItem(world, collect.SourceEntityId);
                    break;
                case DropItemIntent drop:
                    {
                        var state = world.Entities.Get(drop.SourceEntityId);
                        var inv = state?.Get<InventoryState>();
                        if (inv != null && drop.SlotIndex >= 0 && drop.SlotIndex < inv.Slots.Length)
                            DropItem(world, drop.SourceEntityId, inv.Slots[drop.SlotIndex], drop.Amount);
                        break;
                    }
                case InventoryUseIntent use:
                    {
                        var state = world.Entities.Get(use.SourceEntityId);
                        var inv = state?.Get<InventoryState>();
                        if (inv != null && use.SlotIndex >= 0 && use.SlotIndex < inv.Slots.Length)
                            UseItem(world, use.SourceEntityId, use.SlotIndex, inv.Slots[use.SlotIndex]);
                        break;
                    }
                case InventorySwapIntent swap:
                    {
                        var state = world.Entities.Get(swap.SourceEntityId);
                        var inv = state?.Get<InventoryState>();
                        var trade = state?.Get<TradeState>();
                        if (inv == null || inv.Slots[swap.SlotOld].ItemId == Guid.Empty) break;
                        if (swap.SlotOld == swap.SlotNew) break;
                        if (trade?.Partner != null) break;
                        (inv.Slots[swap.SlotOld], inv.Slots[swap.SlotNew]) = (inv.Slots[swap.SlotNew], inv.Slots[swap.SlotOld]);
                        world.Dirty.Mark<InventoryState>(swap.SourceEntityId);
                        tick.Events.Emit(new InventorySwappedEvent
                        {
                            EntityId = swap.SourceEntityId.Value,
                            SlotOld = swap.SlotOld,
                            SlotNew = swap.SlotNew
                        });
                        break;
                    }
            }
        }

        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case InventoryUseItemEvent use:
                    {
                        var playerE = world.Entities.Get(new EntityId(use.EntityId));
                        var playerInv = playerE?.Get<InventoryState>();
                        if (playerInv != null && use.SlotIndex >= 0 && use.SlotIndex < playerInv.Slots.Length)
                            UseItem(world, new EntityId(use.EntityId), use.SlotIndex, playerInv.Slots[use.SlotIndex]);
                        break;
                    }
                case InventoryTakeItemEvent take:
                    {
                        var playerE = world.Entities.Get(new EntityId(take.EntityId));
                        var playerInv = playerE?.Get<InventoryState>();
                        if (playerInv != null && take.SlotIndex >= 0 && take.SlotIndex < playerInv.Slots.Length)
                            TakeItem(world, new EntityId(take.EntityId), playerInv.Slots[take.SlotIndex], take.Amount);
                        break;
                    }
                case InventoryGiveItemEvent give:
                    {
                        var item = _catalog.Items.Get(give.ItemId);
                        if (item != null)
                            GiveItem(world, new EntityId(give.EntityId), item, give.Amount);
                        break;
                    }
                case ItemUsedEvent use:
                    {
                        var playerId = world.FindPlayerByValue(use.PlayerId);
                        if (playerId == null) continue;
                        var e = world.Entities.Get(playerId.Value)!;
                        var inv = e.Get<InventoryState>()!;
                        var item = _catalog.Items.Get(use.ItemId);
                        if (item == null || item.Type != ItemType.Equipment) continue;
                        var slot = inv.Slots[use.SlotIndex];
                        if (slot.ItemId == Guid.Empty || slot.ItemId != use.ItemId) continue;
                        TakeItem(world, playerId.Value, slot, 1);
                        break;
                    }
                case ItemEquippedEvent equip when equip.OldItemId.HasValue:
                    {
                        var playerId = world.FindPlayerByValue(equip.PlayerId);
                        if (playerId == null) continue;
                        var e = world.Entities.Get(playerId.Value)!;
                        var inv = e.Get<InventoryState>()!;
                        var pos = e.Get<Position>()!;
                        var map = world.Maps.Get(pos.MapId)!;

                        var oldItem = _catalog.Items.Get(equip.OldItemId.Value);
                        if (oldItem == null) continue;
                        if (!GiveItem(world, playerId.Value, oldItem, 1))
                        {
                            if (map.GroundItems.Count == Config.MaxMapItems) continue;
                            map.GroundItems.Add(new GroundItem(equip.OldItemId.Value, 1,
                                pos.X, pos.Y));
                        }
                        break;
                    }
            }
        }
    }

    public bool GiveItem(World world, EntityId entityId, Item item, short amount)
    {
        if (item == null) return false;

        var e = world.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;

        var slotItem = inv.Find(item.Id);
        var slotEmpty = Array.Find(inv.Slots, x => x.ItemId == Guid.Empty);
        var slotEmptyIndex = slotEmpty != null ? Array.IndexOf(inv.Slots, slotEmpty) : -1;

        if (slotEmptyIndex == -1) return false;
        if (amount == 0) amount = 1;

        if (slotItem != null && item.Stackable)
            slotItem.Amount += amount;
        else
        {
            var emptySlot = inv.Slots[slotEmptyIndex];
            emptySlot.ItemId = item.Id;
            emptySlot.Amount = item.Stackable ? amount : (byte)1;
        }

        world.Dirty.Mark<InventoryState>(entityId);
        return true;
    }

    private void TakeItem(World world, EntityId entityId, ItemSlot slot, short amount)
    {
        var e = world.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>();
        var inv = e.Get<InventoryState>()!;

        if (slot == null) return;
        if (amount <= 0) amount = 1;

        if (amount == slot.Amount)
        {
            var slotIndex = Array.IndexOf(inv.Slots, slot);
            slot.ItemId = Guid.Empty;
            slot.Amount = 0;

            if (hotbar != null)
            {
                var hotbarSlot = hotbar.Find(SlotType.Item, (short)slotIndex);
                if (hotbarSlot != null)
                {
                    hotbarSlot.Type = SlotType.None;
                    hotbarSlot.Slot = 0;
                    world.Dirty.Mark<HotbarState>(entityId);
                }
            }
        }
        else
            slot.Amount -= amount;

        world.Dirty.Mark<InventoryState>(entityId);
    }

    private void DropItem(World world, EntityId entityId, ItemSlot slot, short amount)
    {
        var e = world.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>();
        var map = world.Maps.Get(pos.MapId)!;

        if (map.GroundItems.Count == Config.MaxMapItems) return;
        if (slot.ItemId == Guid.Empty) return;
        var item = _catalog.Items.Get(slot.ItemId);
        if (item == null || item.Bind == BindOn.Pickup) return;
        if (trade?.Partner != null) return;

        if (amount > slot.Amount) amount = slot.Amount;

        map.GroundItems.Add(new GroundItem(slot.ItemId, amount, pos.X, pos.Y));
        TakeItem(world, entityId, slot, amount);
    }

    private void UseItem(World world, EntityId entityId, int slotIndex, ItemSlot slot)
    {
        var e = world.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;
        var vitals = e.Get<Vitals>()!;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>();
        var catalog = DefinitionCatalog.Instance;

        var item = _catalog.Items.Get(slot.ItemId);
        if (item == null) return;
        if (trade?.Partner != null) return;

        if (stats.Level < item.ReqLevel)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You do not have the level required to use this item.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (item.ReqClassId.HasValue && appearance.ClassId != item.ReqClassId.Value)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You can not use this item.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (item.Type == ItemType.Equipment)
        {
            world.CurrentTick?.Events.Emit(new ItemUsedEvent
            {
                PlayerId = entityId.Value,
                SlotIndex = slotIndex,
                ItemId = item.Id
            });
        }
        else if (item.Type == ItemType.Potion)
        {
            var hadEffect = false;
            world.CurrentTick?.Events.Emit(new XpAwardedEvent { EntityId = entityId.Value, Amount = item.PotionExperience });

            for (byte i = 0; i < (byte)Vital.Count; i++)
            {
                var current = i == 0 ? vitals.Hp : vitals.Mp;
                var max = i == 0 ? vitals.MaxHp : vitals.MaxMp;

                if (current < max && item.PotionVital[i] != 0) hadEffect = true;

                current += item.PotionVital[i];
                if (current < 0) current = 0;
                if (current > max) current = max;
                if (i == 0) vitals.Hp = current; else vitals.Mp = current;
            }

            world.Dirty.Mark<Vitals>(entityId);

            if (vitals.Hp == 0)
                world.CurrentTick?.Events.Emit(new EntityDiedEvent { EntityId = entityId.Value, EntityIsPlayer = true, SourceId = null, SourceIsPlayer = null });

            if (item.PotionExperience > 0 || hadEffect) TakeItem(world, entityId, slot, 1);
        }
    }

    private void CollectItem(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;

        var mapItem = map.HasItem(pos.X, pos.Y);
        if (mapItem == null) return;

        var item = _catalog.Items.Get(mapItem.ItemId);
        if (item == null) return;

        if (GiveItem(world, entityId, item, mapItem.Amount))
        {
            map.GroundItems.Remove(mapItem);
        }
    }
}
