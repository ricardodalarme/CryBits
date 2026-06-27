using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
[MemoryPackUnion(0, typeof(NoAttribute))]
[MemoryPackUnion(1, typeof(BlockedTile))]
[MemoryPackUnion(2, typeof(WarpTile))]
[MemoryPackUnion(3, typeof(SpawnTile))]
[MemoryPackUnion(4, typeof(ItemTile))]
public abstract partial record class TileAttributeUnion;

[MemoryPackable]
public sealed partial record class NoAttribute : TileAttributeUnion;

[MemoryPackable]
public sealed partial record class BlockedTile : TileAttributeUnion;

[MemoryPackable]
public sealed partial record class WarpTile(
    Guid TargetMapId,
    int TargetX,
    int TargetY
) : TileAttributeUnion;

[MemoryPackable]
public sealed partial record class SpawnTile(byte Zone) : TileAttributeUnion;

[MemoryPackable]
public sealed partial record class ItemTile(
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
