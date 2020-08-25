using System;

namespace Entities
{
    [Serializable]
    public class Entity
    {
        // Informações de todos os dados
        public Guid ID;

        public Entity(Guid ID)
        {
            this.ID = ID;
        }
    }
}