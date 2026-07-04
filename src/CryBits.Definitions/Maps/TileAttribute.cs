using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
[MemoryPackUnion(0, typeof(NoAttribute))]
[MemoryPackUnion(1, typeof(BlockedTile))]
[MemoryPackUnion(2, typeof(WarpTile))]
[MemoryPackUnion(3, typeof(SpawnTile))]
[MemoryPackUnion(4, typeof(ItemTile))]
public abstract partial record TileAttributeUnion;

[MemoryPackable]
public sealed partial record NoAttribute : TileAttributeUnion;

[MemoryPackable]
public sealed partial record BlockedTile : TileAttributeUnion;

[MemoryPackable]
public sealed partial record WarpTile(
    Guid TargetMapId,
    int TargetX,
    int TargetY
) : TileAttributeUnion;

[MemoryPackable]
public sealed partial record SpawnTile(byte Zone) : TileAttributeUnion;

[MemoryPackable]
public sealed partial record ItemTile(
    Guid ItemId,
    int Amount
) : TileAttributeUnion;

public enum TileAttribute
{
    None,
    Block,
    Warp,
    Item
}
