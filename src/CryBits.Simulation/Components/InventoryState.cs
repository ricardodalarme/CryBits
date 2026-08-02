using CryBits.Definitions.Slots;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record InventoryState(ItemSlot[] Slots)
{
    public byte CountFreeSlots()
    {
        byte count = 0;
        foreach (var t in Slots)
            if (t.ItemId == Guid.Empty)
                count++;

        return count;
    }
}
