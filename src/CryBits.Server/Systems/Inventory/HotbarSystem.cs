using CryBits.Definitions.Items;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using CryBits.Server.World;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Inventory;

internal sealed class HotbarSystem(InventorySystem inventorySystem) : ISimulationSystem
{
    public static HotbarSystem Instance { get; } = new(InventorySystem.Instance);

    internal void Add(EntityId entityId, short hotbarSlot, SlotType type, short slot)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;
        var inv = e.Get<InventoryState>()!;

        if (slot != 0 && hotbar.Find(type, slot) != null) return;

        hotbar.Slots[hotbarSlot].Slot = slot;
        hotbar.Slots[hotbarSlot].Type = type;
        GameWorld.Current.Dirty.Mark<HotbarState>(entityId);
    }

    internal void Change(EntityId entityId, short slotOld, short slotNew)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;

        if (slotOld < 0 || slotNew < 0) return;
        if (slotOld == slotNew) return;
        if (hotbar.Slots[slotOld].Slot == 0) return;

        (hotbar.Slots[slotOld], hotbar.Slots[slotNew]) = (hotbar.Slots[slotNew], hotbar.Slots[slotOld]);
        GameWorld.Current.Dirty.Mark<HotbarState>(entityId);
    }

    internal void Use(EntityId entityId, short hotbarSlot)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;
        var inv = e.Get<InventoryState>()!;

        switch (hotbar.Slots[hotbarSlot].Type)
        {
            case SlotType.Item:
                var invSlot = hotbar.Slots[hotbarSlot].Slot;
                inventorySystem.UseItem(entityId, invSlot, inv.Slots[invSlot]);
                break;
        }
    }

    internal void SyncInventorySwap(EntityId entityId, short slotOld, short slotNew)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>()!;

        var hotbarSlot = hotbar.Find(SlotType.Item, slotOld);
        if (hotbarSlot == null) return;

        hotbarSlot.Slot = slotNew;
        GameWorld.Current.Dirty.Mark<HotbarState>(entityId);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case HotbarAddIntent add:
                    Add(add.SourceEntityId, add.HotbarSlot, add.Type, add.Slot);
                    break;
                case HotbarSwapIntent swap:
                    Change(swap.SourceEntityId, swap.SlotOld, swap.SlotNew);
                    break;
                case HotbarUseIntent use:
                    Use(use.SourceEntityId, use.Slot);
                    break;
            }
        }
    }
}
