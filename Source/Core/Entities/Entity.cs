using System;
using System.Collections;

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

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        bool IEquatable<Entity>.Equals(Entity other)
        {
            if (other == null) return false;
            return (other).ID == ID;
        }
    }
}
