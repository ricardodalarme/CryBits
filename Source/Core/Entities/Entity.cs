using System;

namespace CryBits.Entities
{
    public class Entity
    {
        public Guid ID;
        public string Name { get; set; }

        public Entity(Guid ID)
        {
            this.ID = ID;
        }

        public override string ToString() => Name;
    }
}
