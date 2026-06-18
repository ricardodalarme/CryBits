using CryBits.Definitions.Common;
using MemoryPack;

namespace CryBits.Client.Framework.Entities.Tile;

[MemoryPackable]
public partial class TileData
{
    public byte Attribute { get; set; }
    public bool[] Block { get; set; } = new bool[(byte)Direction.Count];
}
