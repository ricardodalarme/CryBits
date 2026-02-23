using CryBits.Entities;
using CryBits.Enums;

namespace CryBits.Server.ECS.Components;

/// <summary>Player equipped items, one slot per <see cref="Equipment"/> type.</summary>
internal sealed class EquipmentComponent : ECS.IComponent
{
    public Item?[] Slots = new Item?[(byte)Equipment.Count];
}
