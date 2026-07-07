using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class InventorySystem : ISimulationSystem
{
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
                        var inv = world.Get<InventoryState>(drop.SourceEntityId);
                        if (inv != null && drop.SlotIndex >= 0 && drop.SlotIndex < inv.Slots.Length)
                            DropItem(world, tick, drop.SourceEntityId, drop.SlotIndex, drop.Amount);
                        break;
                    }
                case InventoryUseIntent use:
                    {
                        var inv = world.Get<InventoryState>(use.SourceEntityId);
                        if (inv != null && use.SlotIndex >= 0 && use.SlotIndex < inv.Slots.Length)
                            UseItem(world, tick, use.SourceEntityId, use.SlotIndex, inv.Slots[use.SlotIndex]);
                        break;
                    }
                case InventorySwapIntent swap:
                    {
                        var inv = world.Get<InventoryState>(swap.SourceEntityId);
                        if (inv == null || inv.Slots[swap.SlotOld].ItemId == Guid.Empty) break;
                        if (swap.SlotOld == swap.SlotNew) break;
                        var newSlots = (ItemSlot[])inv.Slots.Clone();
                        (newSlots[swap.SlotOld], newSlots[swap.SlotNew]) = (newSlots[swap.SlotNew], newSlots[swap.SlotOld]);
                        world.Set(swap.SourceEntityId, new InventoryState(newSlots));
                        tick.Events.Emit(new InventorySwappedEvent(tick.TickNumber, swap.SourceEntityId, swap.SlotOld, swap.SlotNew));
                        break;
                    }
                case TradeCommitIntent commit:
                    {
                        if (!world.IsAlive(commit.SourceEntityId) || !world.IsAlive(commit.PartnerId)) break;

                        var sourceInv = world.Get<InventoryState>(commit.SourceEntityId);
                        var partnerInv = world.Get<InventoryState>(commit.PartnerId);
                        if (sourceInv == null || partnerInv == null) break;

                        // Authoritative Inventory Transfer (sim level)
                        // Take source items & give to partner
                        foreach (var item in commit.SourceItems)
                        {
                            var invItem = world.Catalog.Items.Get(item.ItemId);
                            if (invItem != null)
                            {
                                TakeItem(world, commit.SourceEntityId, item.SlotNum, item.Amount);
                                GiveItem(world, commit.PartnerId, invItem, item.Amount);
                            }
                        }

                        // Take partner items & give to source
                        foreach (var item in commit.PartnerItems)
                        {
                            var invItem = world.Catalog.Items.Get(item.ItemId);
                            if (invItem != null)
                            {
                                TakeItem(world, commit.PartnerId, item.SlotNum, item.Amount);
                                GiveItem(world, commit.SourceEntityId, invItem, item.Amount);
                            }
                        }

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
                        var playerInv = world.Get<InventoryState>(use.PlayerId);
                        if (playerInv != null && use.SlotIndex >= 0 && use.SlotIndex < playerInv.Slots.Length)
                            UseItem(world, tick, use.PlayerId, use.SlotIndex, playerInv.Slots[use.SlotIndex]);
                        break;
                    }
                case ItemTakenEvent take:
                    {
                        var playerInv = world.Get<InventoryState>(take.EntityId);
                        if (playerInv != null && take.SlotIndex >= 0 && take.SlotIndex < playerInv.Slots.Length)
                            TakeItem(world, take.EntityId, take.SlotIndex, take.Amount);
                        break;
                    }
                case ItemGivenEvent give:
                    {
                        var item = world.Catalog.Items.Get(give.ItemId);
                        if (item != null)
                            GiveItem(world, give.EntityId, item, give.Amount);
                        break;
                    }
                case ItemEquippedEvent equip when equip.OldItemId.HasValue:
                    {
                        if (!world.Has<PlayerTag>(equip.PlayerId)) continue;
                        var pos = world.Get<Position>(equip.PlayerId);
                        if (pos == null) continue;
                        var oldItem = world.Catalog.Items.Get(equip.OldItemId.Value);
                        if (oldItem == null) continue;
                        if (!GiveItem(world, equip.PlayerId, oldItem, 1))
                        {
                            tick.Events.Emit(new LootDroppedEvent(tick.TickNumber, pos.MapId, pos.X, pos.Y, equip.OldItemId.Value, 1, tick.TickNumber + GroundItemDespawnTicks));
                        }
                        break;
                    }
            }
        }
    }

    private bool GiveItem(World world, EntityId entityId, Item item, short amount)
    {
        if (item == null) return false;

        var inv = world.Get<InventoryState>(entityId);
        if (inv == null) return false;

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

        var newSlots = (ItemSlot[])inv.Slots.Clone();
        if (stackSlot != null && item.Stackable)
            newSlots[stackSlot.Value] = newSlots[stackSlot.Value] with { Amount = (short)(newSlots[stackSlot.Value].Amount + amount) };
        else
        {
            newSlots[emptySlot.Value] = new ItemSlot(item.Id, item.Stackable ? amount : (byte)1);
        }

        world.Set(entityId, new InventoryState(newSlots));
        return true;
    }

    private void TakeItem(World world, EntityId entityId, int slotIndex, short amount)
    {
        if (!world.IsAlive(entityId)) return;
        var hotbar = world.Get<HotbarState>(entityId);
        var inv = world.Get<InventoryState>(entityId)!;

        if (amount <= 0) amount = 1;

        var newSlots = (ItemSlot[])inv.Slots.Clone();
        if (amount == newSlots[slotIndex].Amount)
        {
            newSlots[slotIndex] = new ItemSlot(Guid.Empty, 0);

            if (hotbar != null)
            {
                var newHotbarSlots = (HotbarSlot[])hotbar.Slots.Clone();
                for (var h = 0; h < newHotbarSlots.Length; h++)
                {
                    if (newHotbarSlots[h].Type == SlotType.Item && newHotbarSlots[h].Slot == slotIndex)
                    {
                        newHotbarSlots[h] = new HotbarSlot(Type: SlotType.None, Slot: 0);
                        world.Set(entityId, new HotbarState(newHotbarSlots));
                        break;
                    }
                }
            }
        }
        else
            newSlots[slotIndex] = newSlots[slotIndex] with { Amount = (short)(newSlots[slotIndex].Amount - amount) };

        world.Set(entityId, new InventoryState(newSlots));
    }

    private void DropItem(World world, Tick tick, EntityId entityId, int slotIndex, short amount)
    {
        if (!world.IsAlive(entityId)) return;
        var inv = world.Get<InventoryState>(entityId)!;
        var pos = world.Get<Position>(entityId)!;

        if (inv.Slots[slotIndex].ItemId == Guid.Empty) return;
        var item = world.Catalog.Items.Get(inv.Slots[slotIndex].ItemId);
        if (item == null || item.Bind == BindOn.Pickup) return;

        if (amount > inv.Slots[slotIndex].Amount) amount = inv.Slots[slotIndex].Amount;

        tick.Events.Emit(new LootDroppedEvent(tick.TickNumber, pos.MapId, pos.X, pos.Y, inv.Slots[slotIndex].ItemId, amount, tick.TickNumber + GroundItemDespawnTicks));
        TakeItem(world, entityId, slotIndex, amount);
    }

    private void UseItem(World world, Tick tick, EntityId entityId, int slotIndex, ItemSlot slot)
    {
        if (!world.IsAlive(entityId)) return;
        var level = world.Get<LevelComponent>(entityId)!;
        var vitals = world.Get<Vitals>(entityId)!;
        var appearance = world.Get<PlayerAppearance>(entityId)!;

        var item = world.Catalog.Items.Get(slot.ItemId);
        if (item == null) return;

        if (level.Level < item.ReqLevel)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You do not have the level required to use this item.", ChatColors.White));
            return;
        }

        if (item.ReqClassId.HasValue && appearance.ClassId != item.ReqClassId.Value)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You can not use this item.", ChatColors.White));
            return;
        }

        if (item.Type == ItemType.Equipment)
        {
            tick.Events.Emit(new ItemUsedEvent(tick.TickNumber, entityId, slotIndex, item.Id, false));
        }
        else if (item.Type == ItemType.Potion)
        {
            var hadEffect = false;
            tick.Events.Emit(new XpAwardedEvent(tick.TickNumber, entityId, item.PotionExperience));

            var newHp = vitals.Hp;
            var newMp = vitals.Mp;
            for (byte i = 0; i < (byte)Vital.Count; i++)
            {
                var current = i == 0 ? newHp : newMp;
                var max = i == 0 ? vitals.MaxHp : vitals.MaxMp;

                if (current < max && item.PotionVital[i] != 0) hadEffect = true;

                current += item.PotionVital[i];
                if (current < 0) current = 0;
                if (current > max) current = max;
                if (i == 0) newHp = (short)current; else newMp = (short)current;
            }

            world.Set(entityId, new Vitals(Hp: newHp, Mp: newMp, MaxHp: vitals.MaxHp, MaxMp: vitals.MaxMp));

            if (newHp == 0)
                tick.Events.Emit(new PlayerDiedEvent(tick.TickNumber, entityId, null));

            if (item.PotionExperience > 0 || hadEffect) TakeItem(world, entityId, slotIndex, 1);
        }
    }

    private void CollectItem(World world, EntityId entityId)
    {
        var pos = world.Get<Position>(entityId);
        if (pos == null) return;

        var groundEntityId = ChunkGrid.FindAt<GroundItem>(world, pos.MapId, pos.X, pos.Y);
        if (groundEntityId == null) return;

        if (!world.IsAlive(groundEntityId.Value)) return;
        var comp = world.Get<GroundItem>(groundEntityId.Value)!;
        var item = world.Catalog.Items.Get(comp.ItemDefId);
        if (item == null) return;

        if (GiveItem(world, entityId, item, comp.Amount))
            world.Destroy(groundEntityId.Value);
    }

}
