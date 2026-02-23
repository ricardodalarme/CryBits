using CryBits.Entities.Slots;
using static CryBits.Globals;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Local player's inventory slots. Only present on the entity with <see cref="LocalPlayerTag"/>.
/// </summary>
public sealed class InventoryComponent : IComponent
{
    public ItemSlot?[] Slots { get; set; } = new ItemSlot?[MaxInventory];
}
