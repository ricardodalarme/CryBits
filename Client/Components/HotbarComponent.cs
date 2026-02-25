using CryBits.Entities.Slots;

namespace CryBits.Client.Components;

/// <summary>
/// A component that holds the player's hotbar. Each slot can be empty (null) or contain a hotbar slot reference.
/// </summary>
internal struct HotbarComponent()
{
    public HotbarSlot?[] Slots = new HotbarSlot?[Globals.MaxHotbar];
}
