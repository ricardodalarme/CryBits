using CryBits.Definitions.Slots;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class InventoryState(ItemSlot[] Slots)
{
    public byte CountFreeSlots()
    {
        byte count = 0;
        for (var i = 0; i < Slots.Length; i++)
            if (Slots[i].ItemId == Guid.Empty) count++;
        return count;
    }
}
