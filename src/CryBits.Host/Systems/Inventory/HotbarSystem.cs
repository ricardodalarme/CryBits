using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Systems.Inventory;

internal sealed class HotbarSystem(InventorySystem inventorySystem) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case HotbarAddIntent add:
                    Add(world, add.SourceEntityId, add.HotbarSlot, add.Type, add.Slot);
                    break;
                case HotbarSwapIntent swap:
                    Change(world, swap.SourceEntityId, swap.SlotOld, swap.SlotNew);
                    break;
                case HotbarUseIntent use:
                    Use(world, use.SourceEntityId, use.Slot);
                    break;
            }
        }

        foreach (var ev in tick.Events.Events)
        {
            if (ev is InventorySwappedEvent swapped)
                SyncInventorySwap(world, new EntityId(swapped.EntityId), swapped.SlotOld, swapped.SlotNew);
        }
    }

    private void Add(World world, EntityId entityId, short hotbarSlot, SlotType type, short slot)
    {
        var e = world.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;

        if (hotbarSlot >= hotbar.Slots.Length) return;

        hotbar.Slots[hotbarSlot].Type = type;
        hotbar.Slots[hotbarSlot].Slot = slot;
        world.Dirty.Mark<HotbarState>(entityId);
    }

    private void Change(World world, EntityId entityId, short slotOld, short slotNew)
    {
        var e = world.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;

        (hotbar.Slots[slotOld], hotbar.Slots[slotNew]) = (hotbar.Slots[slotNew], hotbar.Slots[slotOld]);
        world.Dirty.Mark<HotbarState>(entityId);
    }

    private void Use(World world, EntityId entityId, short hotbarSlot)
    {
        var e = world.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;
        var inv = e.Get<InventoryState>()!;

        switch (hotbar.Slots[hotbarSlot].Type)
        {
            case SlotType.Item:
                var invSlot = hotbar.Slots[hotbarSlot].Slot;
                inventorySystem.UseItem(world, entityId, invSlot, inv.Slots[invSlot]);
                break;
        }
    }

    private void SyncInventorySwap(World world, EntityId entityId, short slotOld, short slotNew)
    {
        var e = world.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;

        var hotbarSlot = hotbar.Find(SlotType.Item, slotOld);
        if (hotbarSlot == null) return;

        hotbarSlot.Slot = slotNew;
        world.Dirty.Mark<HotbarState>(entityId);
    }
}
