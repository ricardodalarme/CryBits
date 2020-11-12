using System;

namespace CryBits.Entities
{
    [Serializable]
    public class Entity : IEquatable<Entity>
    {
        public Guid ID;
        public string Name { get; set; } = string.Empty;

        public Entity(Guid ID)
        {
            this.ID = ID;
        }

        public override string ToString() => Name;

        public override int GetHashCode() => ID.GetHashCode();

        bool IEquatable<Entity>.Equals(Entity other) => other == null && (other).ID == ID;
    }
}
