using CryBits.Definitions.Items;
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

        hotbar.Slots[hotbarSlot].Type = type;
        hotbar.Slots[hotbarSlot].Slot = slot;
        world.MarkDirty<HotbarState>(entityId);
    }

    private void Change(World world, EntityId entityId, short slotOld, short slotNew)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var hotbar = e.Get<HotbarState>()!;

        (hotbar.Slots[slotOld], hotbar.Slots[slotNew]) = (hotbar.Slots[slotNew], hotbar.Slots[slotOld]);
        world.MarkDirty<HotbarState>(entityId);
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
                tick.Events.Emit(new ItemUsedEvent { PlayerId = entityId, SlotIndex = invSlot, DirectUse = true });
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

        hotbar.Slots[foundSlot.Value].Slot = slotNew;
        world.MarkDirty<HotbarState>(entityId);
    }
}
