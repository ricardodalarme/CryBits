using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public sealed partial record MapChunk(
    short X,
    short Y,
    long Version,
    TileData[,]? Tiles = null,
    WeatherType? WeatherOverride = null,
    FogConfig? FogOverride = null,
    byte? LightingOverride = null
)
{
    [MemoryPackConstructor]
    public MapChunk() : this(0, 0, 0) { }

    public MapChunk WithNextVersion() => this with { Version = Version + 1 };
}
