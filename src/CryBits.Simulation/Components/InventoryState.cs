using CryBits.Definitions.Slots;
using static CryBits.Definitions.Globals;

namespace CryBits.Simulation.Components;

public sealed class InventoryState
{
    public ItemSlot[] Slots { get; set; } = new ItemSlot[MaxInventory];

    public byte CountFreeSlots()
    {
        byte count = 0;
        for (var i = 0; i < Slots.Length; i++)
            if (Slots[i].ItemId == Guid.Empty) count++;
        return count;
    }
}
