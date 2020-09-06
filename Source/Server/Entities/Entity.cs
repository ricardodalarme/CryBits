using System;

namespace Entities
{
    [Serializable]
    abstract class Entity
    {
        // Informações de todos os dados
        public Guid ID;

        public Entity(Guid ID)
        {
            this.ID = ID;
        }
    }
}