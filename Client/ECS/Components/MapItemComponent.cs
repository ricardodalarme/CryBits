using CryBits.Entities;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// An item dropped on the map floor, visible to the player.
/// Entities with this component are created/destroyed when the server sends
/// map-item sync packets.
/// </summary>
public sealed class MapItemComponent : IComponent
{
    /// <summary>Server-side slot index in the map's item array.</summary>
    public byte SlotIndex { get; set; }

    /// <summary>Null means the slot is empty (entity should be destroyed).</summary>
    public Item? Item { get; set; }

    public byte TileX { get; set; }
    public byte TileY { get; set; }
}
