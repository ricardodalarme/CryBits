using System;
using System.Collections.Generic;
using CryBits.Enums;
using CryBits.Extensions;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Entities;

/// <summary>Game item definition.</summary>
[Serializable]
public class Item : Entity
{
    /// <summary>Registered items keyed by id.</summary>
    public static Dictionary<Guid, Item> List = [];

    public string Description { get; set; } = string.Empty;
    public short Texture { get; set; }
    public ItemType Type { get; set; }
    public bool Stackable { get; set; }
    public BindOn Bind { get; set; }
    public Rarity Rarity { get; set; }

    public short ReqLevel { get; set; }
    private Guid _reqClass;

    public Class ReqClass
    {
        get => Class.List.Get(_reqClass);
        set => _reqClass = value.GetId();
    }

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