using CryBits.Definitions.Items;

namespace CryBits.Client.Components.Equipment;

/// <summary>
/// Equipment slots for a character (Weapon, Armor, Helmet, Shield, Amulet).
/// </summary>
internal struct EquipmentComponent()
{
    /// <summary>Equipped items indexed by CryBits.Definitions.Items.Equipment.</summary>
    public Item?[] Slots = new Item?[(byte)CryBits.Definitions.Items.Equipment.Count];
}
