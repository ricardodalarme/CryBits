using CryBits.Definitions.Common;
using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public partial class MapAttribute
{
    public byte Type { get; set; }
    public string Data1 { get; set; } = string.Empty;
    public short Data2 { get; set; }
    public short Data3 { get; set; }
    public short Data4 { get; set; }
    public byte Zone { get; set; }
    public bool[] Block { get; set; } = new bool[(byte)Direction.Count];
}
