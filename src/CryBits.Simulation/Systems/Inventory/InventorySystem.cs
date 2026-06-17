using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using static CryBits.Simulation.SimulationConstants;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class InventorySystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case CollectItemIntent collect:
                    CollectItem(world, tick, collect.SourceEntityId);
                    break;
                case DropItemIntent drop:
                    {
                        var state = world.Entities.Get(drop.SourceEntityId);
                        var inv = state?.Get<InventoryState>();
                        if (inv != null && drop.SlotIndex >= 0 && drop.SlotIndex < inv.Slots.Length)
                            DropItem(world, tick, drop.SourceEntityId, drop.SlotIndex, drop.Amount);
                        break;
                    }
                case InventoryUseIntent use:
                    {
                        var state = world.Entities.Get(use.SourceEntityId);
                        var inv = state?.Get<InventoryState>();
                        if (inv != null && use.SlotIndex >= 0 && use.SlotIndex < inv.Slots.Length)
                            UseItem(world, tick, use.SourceEntityId, use.SlotIndex, inv.Slots[use.SlotIndex]);
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
                            EntityId = swap.SourceEntityId,
                            SlotOld = swap.SlotOld,
                            SlotNew = swap.SlotNew
                        });
                        break;
                    }
            }
        }

        var events = tick.Events.Events;
        for (var i = 0; i < events.Count; i++)
        {
            var ev = events[i];
            switch (ev)
            {
                case ItemUsedEvent use when use.DirectUse:
                    {
                        var playerE = world.Entities.Get(use.PlayerId);
                        var playerInv = playerE?.Get<InventoryState>();
                        if (playerInv != null && use.SlotIndex >= 0 && use.SlotIndex < playerInv.Slots.Length)
                            UseItem(world, tick, use.PlayerId, use.SlotIndex, playerInv.Slots[use.SlotIndex]);
                        break;
                    }
                case ItemTakenEvent take:
                    {
                        var playerE = world.Entities.Get(take.EntityId);
                        var playerInv = playerE?.Get<InventoryState>();
                        if (playerInv != null && take.SlotIndex >= 0 && take.SlotIndex < playerInv.Slots.Length)
                            TakeItem(world, take.EntityId, take.SlotIndex, take.Amount);
                        break;
                    }
                case ItemGivenEvent give:
                    {
                        var item = catalog.Items.Get(give.ItemId);
                        if (item != null)
                            GiveItem(world, give.EntityId, item, give.Amount);
                        break;
                    }
                case ItemEquippedEvent equip when equip.OldItemId.HasValue:
                    {
                        var playerId = world.FindPlayer(equip.PlayerId);
                        if (playerId == null) continue;
                        var e = world.Entities.Get(playerId.Value)!;
                        var inv = e.Get<InventoryState>()!;
                        var pos = e.Get<Position>()!;
                        var map = world.Maps.Get(pos.MapId)!;

                        var oldItem = catalog.Items.Get(equip.OldItemId.Value);
                        if (oldItem == null) continue;
                        if (!GiveItem(world, playerId.Value, oldItem, 1))
                        {
                            tick.Events.Emit(new LootDroppedEvent
                            {
                                MapId = pos.MapId,
                                X = pos.X,
                                Y = pos.Y,
                                ItemId = equip.OldItemId.Value,
                                Amount = 1,
                                DespawnTick = tick.TickNumber + GroundItemDespawnTicks
                            });
                        }
                        break;
                    }
            }
        }
    }

    private bool GiveItem(World world, EntityId entityId, Item item, short amount)
    {
        if (item == null) return false;

        var e = world.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;

        int? stackSlot = null;
        int? emptySlot = null;
        for (var i = 0; i < inv.Slots.Length; i++)
        {
            if (inv.Slots[i].ItemId == item.Id)
                stackSlot = i;
            if (inv.Slots[i].ItemId == Guid.Empty && emptySlot == null)
                emptySlot = i;
        }

        if (emptySlot == null) return false;
        if (amount == 0) amount = 1;

        if (stackSlot != null && item.Stackable)
            inv.Slots[stackSlot.Value].Amount += amount;
        else
        {
            inv.Slots[emptySlot.Value].ItemId = item.Id;
            inv.Slots[emptySlot.Value].Amount = item.Stackable ? amount : (byte)1;
        }

        world.Dirty.Mark<InventoryState>(entityId);
        return true;
    }

    private void TakeItem(World world, EntityId entityId, int slotIndex, short amount)
    {
        var e = world.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>();
        var inv = e.Get<InventoryState>()!;

        if (amount <= 0) amount = 1;

        if (amount == inv.Slots[slotIndex].Amount)
        {
            inv.Slots[slotIndex].ItemId = Guid.Empty;
            inv.Slots[slotIndex].Amount = 0;

            if (hotbar != null)
            {
                for (var h = 0; h < hotbar.Slots.Length; h++)
                {
                    if (hotbar.Slots[h].Type == SlotType.Item && hotbar.Slots[h].Slot == slotIndex)
                    {
                        hotbar.Slots[h].Type = SlotType.None;
                        hotbar.Slots[h].Slot = 0;
                        world.Dirty.Mark<HotbarState>(entityId);
                        break;
                    }
                }
            }
        }
        else
            inv.Slots[slotIndex].Amount -= amount;

        world.Dirty.Mark<InventoryState>(entityId);
    }

    private void DropItem(World world, Tick tick, EntityId entityId, int slotIndex, short amount)
    {
        var e = world.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>();
        var map = world.Maps.Get(pos.MapId)!;

        if (inv.Slots[slotIndex].ItemId == Guid.Empty) return;
        var item = catalog.Items.Get(inv.Slots[slotIndex].ItemId);
        if (item == null || item.Bind == BindOn.Pickup) return;
        if (trade?.Partner != null) return;

        if (amount > inv.Slots[slotIndex].Amount) amount = inv.Slots[slotIndex].Amount;

        tick.Events.Emit(new LootDroppedEvent
        {
            MapId = pos.MapId,
            X = pos.X,
            Y = pos.Y,
            ItemId = inv.Slots[slotIndex].ItemId,
            Amount = amount,
            DespawnTick = tick.TickNumber + GroundItemDespawnTicks
        });
        TakeItem(world, entityId, slotIndex, amount);
    }

    private void UseItem(World world, Tick tick, EntityId entityId, int slotIndex, ItemSlot slot)
    {
        var e = world.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;
        var vitals = e.Get<Vitals>()!;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>();

        var item = catalog.Items.Get(slot.ItemId);
        if (item == null) return;
        if (trade?.Partner != null) return;

        if (stats.Level < item.ReqLevel)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You do not have the level required to use this item.", ColorArgb = ChatColors.White });
            return;
        }

        if (item.ReqClassId.HasValue && appearance.ClassId != item.ReqClassId.Value)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You can not use this item.", ColorArgb = ChatColors.White });
            return;
        }

        if (item.Type == ItemType.Equipment)
        {
            tick.Events.Emit(new ItemUsedEvent
            {
                PlayerId = entityId,
                SlotIndex = slotIndex,
                ItemId = item.Id
            });
        }
        else if (item.Type == ItemType.Potion)
        {
            var hadEffect = false;
            tick.Events.Emit(new XpAwardedEvent { EntityId = entityId, Amount = item.PotionExperience });

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
                tick.Events.Emit(new PlayerDiedEvent { EntityId = entityId });

            if (item.PotionExperience > 0 || hadEffect) TakeItem(world, entityId, slotIndex, 1);
        }
    }

    private void CollectItem(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;

        var groundEntityId = map.FindGroundItemEntity(world.Entities, pos.X, pos.Y);
        if (groundEntityId == null) return;

        var groundEntity = world.Entities.Get(groundEntityId.Value)!;
        var comp = groundEntity.Get<GroundItem>()!;
        var item = catalog.Items.Get(comp.ItemDefId);
        if (item == null) return;

        if (GiveItem(world, entityId, item, comp.Amount))
        {
            tick.Events.Emit(new GroundItemRemovedEvent { EntityId = groundEntityId.Value, MapId = map.Id });
            world.Entities.Destroy(groundEntityId.Value);
            map.GroundItemIds.Remove(groundEntityId.Value);
        }
    }
}
