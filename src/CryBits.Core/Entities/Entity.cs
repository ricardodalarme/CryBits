using System;

namespace CryBits.Entities;

[Serializable]
public class Entity(Guid id) : IEquatable<Entity>
{
    public Guid Id { get; } = id;
    public string Name { get; set; } = string.Empty;

    public Entity() : this(Guid.NewGuid())
    {
    }

    public override string ToString() => Name;

    public override int GetHashCode() => Id.GetHashCode();

    bool IEquatable<Entity>.Equals(Entity other) => other != null && other.Id.Equals(Id);
}
