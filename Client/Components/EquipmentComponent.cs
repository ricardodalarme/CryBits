using CryBits.Entities;

namespace CryBits.Client.Components;

/// <summary>
/// Equipment slots for a character (Weapon, Armor, Helmet, Shield, Amulet).
/// </summary>
internal struct EquipmentComponent()
{
    /// <summary>Equipped items indexed by Enums.Equipment.</summary>
    public Item?[] Slots = new Item?[(byte)Enums.Equipment.Count];
}
