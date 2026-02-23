using System;
using CryBits.Entities;
using CryBits.Enums;
using Attr = CryBits.Enums.Attribute;
using Equip = CryBits.Enums.Equipment;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Player-specific data synced by the server.
/// Applies to all player entities — both remote and the local player.
/// </summary>
public sealed class PlayerDataComponent : IComponent
{
    public string Name { get; set; } = string.Empty;
    public short Level { get; set; }
    public Guid MapId { get; set; }
    public short[] Attributes { get; set; } = new short[(byte)Attr.Count];
    public Item?[] EquippedItems { get; set; } = new Item[(byte)Equip.Count];
}
