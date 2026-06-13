using CryBits.Definitions.Slots;
using static CryBits.Definitions.Globals;

namespace CryBits.Simulation.Components;

public sealed class InventoryState
{
    public ItemSlot[] Slots { get; set; } = new ItemSlot[MaxInventory];
}
