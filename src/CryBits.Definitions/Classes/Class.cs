using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using System;
using System.Collections.Generic;

namespace CryBits.Definitions.Classes;

/// <summary>Character class metadata (stats, textures and spawn info).</summary>
[Serializable]
public class Class : Entity
{
    public string Description { get; set; } = string.Empty;
    public IList<short> TextureMale { get; set; } = [];
    public IList<short> TextureFemale { get; set; } = [];
    public Guid SpawnMapId { get; set; }
    public byte SpawnDirection { get; set; }
    public byte SpawnX { get; set; }
    public byte SpawnY { get; set; }
    public short[] Vital { get; set; } = new short[(byte)CryBits.Definitions.Characters.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)CryBits.Definitions.Characters.Attribute.Count];
    public IList<ItemSlot> Item { get; set; } = [];

    public Class()
    {
        Name = "New class";
        TextureMale.Add(1);
        TextureFemale.Add(1);
    }
}
