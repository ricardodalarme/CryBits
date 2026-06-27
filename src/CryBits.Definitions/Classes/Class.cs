using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using MemoryPack;

namespace CryBits.Definitions.Classes;

/// <summary>Character class metadata (stats, textures and spawn info).</summary>
[MemoryPackable]
public partial record class Class : Definition
{
    public string Description { get; set; } = string.Empty;
    public IList<short> TextureMale { get; set; } = [];
    public IList<short> TextureFemale { get; set; } = [];
    public Guid SpawnMapId { get; set; }
    public byte SpawnDirection { get; set; }
    public int SpawnX { get; set; }
    public int SpawnY { get; set; }
    public short[] Vital { get; set; } = new short[(byte)Characters.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)Characters.Attribute.Count];
    public IList<ItemSlot> Item { get; set; } = [];

    public Class()
    {
        Name = "New class";
        TextureMale.Add(1);
        TextureFemale.Add(1);
    }
}
