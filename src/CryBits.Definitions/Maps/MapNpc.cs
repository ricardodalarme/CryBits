using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public partial class MapNpc
{
    public Guid NpcId { get; set; }
    public byte Zone { get; set; }
    public bool Spawn { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
}
