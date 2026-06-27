using MemoryPack;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public readonly partial record struct ChunkCoord(short X, short Y)
{
    public override string ToString() => $"{X},{Y}";

    public static ChunkCoord FromString(string value)
    {
        var parts = value.Split(',');
        return new ChunkCoord(short.Parse(parts[0]), short.Parse(parts[1]));
    }

    public static implicit operator ChunkCoord((short X, short Y) tuple) =>
        new(tuple.X, tuple.Y);

    public void Deconstruct(out short X, out short Y)
    {
        X = this.X;
        Y = this.Y;
    }
}
