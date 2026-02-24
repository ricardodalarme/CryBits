using CryBits.Entities;

namespace CryBits.Client.Components;

/// <summary>
/// Tags an entity as a ground item on the current map.
/// </summary>
internal struct GroundItemComponent(Item item)
{
    public Item Item = item;
}
