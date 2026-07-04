using MemoryPack;

namespace CryBits.Definitions.Common;

[MemoryPackable]
public partial record Definition : IEquatable<Definition>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [MemoryPackConstructor]
    public Definition()
    {
        Id = Guid.NewGuid();
    }

    public Definition(Guid id)
    {
        Id = id;
    }

    public override sealed string ToString() => Name;

    public override int GetHashCode() => Id.GetHashCode();

    bool IEquatable<Definition>.Equals(Definition? other) => other != null && other.Id.Equals(Id);
}
