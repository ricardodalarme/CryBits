using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Entities.Slots;

namespace CryBits.Entities;

/// <summary>Character class metadata (stats, textures and spawn info).</summary>
[Serializable]
public class Class : Entity
{
    /// <summary>Registered classes keyed by id.</summary>
    public static Dictionary<Guid, Class> List = [];

    public string Description { get; set; } = string.Empty;
    public IList<short> TextureMale { get; set; } = [];
    public IList<short> TextureFemale { get; set; } = [];
    public Map.Map SpawnMap { get; set; }
    public byte SpawnDirection { get; set; }
    public byte SpawnX { get; set; }
    public byte SpawnY { get; set; }
    public short[] Vital { get; set; } = new short[(byte)Enums.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
    public IList<ItemSlot> Item { get; set; } = [];

    public Class()
    {
        Name = "New class";
        SpawnMap = Map.Map.List.ElementAt(0).Value;
        TextureMale.Add(1);
        TextureFemale.Add(1);
    }
}