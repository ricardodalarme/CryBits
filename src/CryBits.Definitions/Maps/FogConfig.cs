using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public sealed partial record class FogConfig(byte Texture, sbyte SpeedX, sbyte SpeedY, byte Alpha)
{
    [MemoryPackConstructor]
    public FogConfig() : this(0, 0, 0, 255) { }
}
