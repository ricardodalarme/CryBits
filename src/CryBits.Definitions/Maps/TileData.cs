using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public sealed partial record class TileData(
    byte Texture,
    int SourceX,
    int SourceY,
    bool IsAutoTile,
    TileAttributeUnion Attribute,
    Layer Layer = Layer.Ground
)
{
    public bool IsBlocked => Attribute is BlockedTile;

    public bool IsVisible => Texture > 0;
}
