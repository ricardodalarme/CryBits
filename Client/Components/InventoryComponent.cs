using CryBits.Entities.Slots;

namespace CryBits.Client.Components;

/// <summary>
/// A component that holds the player's inventory. Each slot can be empty (null) or contain an item.
/// </summary>
internal struct InventoryComponent()
{
    public ItemSlot?[] Slots = new ItemSlot?[Globals.MaxInventory];
}
