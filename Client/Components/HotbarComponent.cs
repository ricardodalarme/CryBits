using CryBits.Entities.Slots;

namespace CryBits.Client.Components;

/// <summary>
/// A component that holds the player's hotbar. Each slot can be empty (null) or contain an item.
/// </summary>
internal struct HotbarComponent()
{
    public ItemSlot?[] Slots = new ItemSlot?[Globals.MaxInventory];
}
