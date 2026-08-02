using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public readonly partial record struct ChunkCoord(short X, short Y)
{
    public override string ToString() => $"{X},{Y}";

    public static implicit operator ChunkCoord((short X, short Y) tuple) =>
        new(tuple.X, tuple.Y);
}
