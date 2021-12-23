using System;

namespace CryBits.Entities;

[Serializable]
public class Entity : IEquatable<Entity>
{
    public Guid Id { get; }
    public string Name { get; set; } = string.Empty;

    public Entity()
    {
        Id = Guid.NewGuid();
    }

    public Entity(Guid id)
    {
        Id = id;
    }

    public override string ToString() => Name;

    public override int GetHashCode() => Id.GetHashCode();

    bool IEquatable<Entity>.Equals(Entity other) => other != null && other.Id.Equals(Id);
}