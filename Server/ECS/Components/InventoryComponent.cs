using CryBits.Entities.Slots;
using static CryBits.Globals;

namespace CryBits.Server.ECS.Components;

/// <summary>Player inventory — pre-allocated slot array, mutated in place.</summary>
internal sealed class InventoryComponent : ECS.IComponent
{
    public ItemSlot[] Slots;

    public InventoryComponent()
    {
        Slots = new ItemSlot[MaxInventory];
        for (var i = 0; i < Slots.Length; i++)
            Slots[i] = new ItemSlot(null, 0);
    }
}
