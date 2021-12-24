using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Entities.Slots;

namespace CryBits.Entities;

[Serializable]
public class Class : Entity
{
    // Lista de dados
    public static Dictionary<Guid, Class> List = new();

    // Dados
    public string Description { get; set; } = string.Empty;
    public IList<short> TextureMale { get; set; } = new List<short>();
    public IList<short> TextureFemale { get; set; } = new List<short>();
    public Map.Map SpawnMap { get; set; }
    public byte SpawnDirection { get; set; }
    public byte SpawnX { get; set; }
    public byte SpawnY { get; set; }
    public short[] Vital { get; set; } = new short[(byte)Enums.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
    public IList<ItemSlot> Item { get; set; } = new List<ItemSlot>();

    public Class()
    {
        Name = "New class";
        SpawnMap = Map.Map.List.ElementAt(0).Value;
        TextureMale.Add(1);
        TextureFemale.Add(1);
    }
}