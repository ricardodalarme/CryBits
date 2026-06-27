using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class HotbarSystem : ISimulationSystem
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
                    Use(world, tick, use.SourceEntityId, use.Slot);
                    break;
            }
        }

        var events = tick.Events.Events;
        for (var i = 0; i < events.Count; i++)
        {
            if (events[i] is InventorySwappedEvent swapped)
                SyncInventorySwap(world, swapped.EntityId, swapped.SlotOld, swapped.SlotNew);
        }
    }

    private void Add(World world, EntityId entityId, short hotbarSlot, SlotType type, short slot)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var hotbar = e.Get<HotbarState>()!;

        if (hotbarSlot >= hotbar.Slots.Length) return;

        var newSlots = (HotbarSlot[])hotbar.Slots.Clone();
        newSlots[hotbarSlot] = newSlots[hotbarSlot] with { Type = type, Slot = slot };
        world.Set(entityId, new HotbarState(newSlots));
    }

    private void Change(World world, EntityId entityId, short slotOld, short slotNew)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var hotbar = e.Get<HotbarState>()!;

        var newSlots = (HotbarSlot[])hotbar.Slots.Clone();
        (newSlots[slotOld], newSlots[slotNew]) = (newSlots[slotNew], newSlots[slotOld]);
        world.Set(entityId, new HotbarState(newSlots));
    }

    private void Use(World world, Tick tick, EntityId entityId, short hotbarSlot)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var hotbar = e.Get<HotbarState>()!;

        switch (hotbar.Slots[hotbarSlot].Type)
        {
            case SlotType.Item:
                var invSlot = hotbar.Slots[hotbarSlot].Slot;
                tick.Events.Emit(new ItemUsedEvent(tick.TickNumber, entityId, invSlot, Guid.Empty, true));
                break;
        }
    }

    private void SyncInventorySwap(World world, EntityId entityId, short slotOld, short slotNew)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var hotbar = e.Get<HotbarState>()!;

        int? foundSlot = null;
        for (var i = 0; i < hotbar.Slots.Length; i++)
        {
            if (hotbar.Slots[i].Type == SlotType.Item && hotbar.Slots[i].Slot == slotOld)
            {
                foundSlot = i;
                break;
            }
        }

        if (foundSlot == null) return;

        var newSlots = (HotbarSlot[])hotbar.Slots.Clone();
        newSlots[foundSlot.Value] = newSlots[foundSlot.Value] with { Slot = slotNew };
        world.Set(entityId, new HotbarState(newSlots));
    }
}
