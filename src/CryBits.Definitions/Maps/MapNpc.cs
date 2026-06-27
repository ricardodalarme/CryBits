using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public partial class MapNpc
{
    public Guid NpcId { get; set; }
    public byte Zone { get; set; }
    public bool Spawn { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
