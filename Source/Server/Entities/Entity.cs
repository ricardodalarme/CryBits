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

        // Obtém o ID de algum dado, caso ele não existir retorna um ID zerado
        public static string GetID(Entity Object) => Object == null ? Guid.Empty.ToString() : Object.ID.ToString();
    }
}