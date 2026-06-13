using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using System;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Definitions.Items;

/// <summary>Game item definition.</summary>
[Serializable]
public class Item : Entity
{
    public string Description { get; set; } = string.Empty;
    public short Texture { get; set; }
    public ItemType Type { get; set; }
    public bool Stackable { get; set; }
    public BindOn Bind { get; set; }
    public Rarity Rarity { get; set; }

    public short ReqLevel { get; set; }
    public Guid? ReqClassId { get; set; }

    public int PotionExperience { get; set; }
    public short[] PotionVital { get; set; } = new short[(byte)Vital.Count];

    public byte EquipType { get; set; }
    public short[] EquipAttribute { get; set; } = new short[(byte)Attribute.Count];
    public short WeaponDamage { get; set; }

    public Item()
    {
        Name = "New item";
    }
}
