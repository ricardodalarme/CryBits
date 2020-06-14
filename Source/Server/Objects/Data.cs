using System;

namespace Objects
{
    [Serializable]
    public class Data
    {
        // Informações de todos os dados
        public Guid ID;
        public readonly DateTime CreationTime;

        public Data(Guid ID)
        {
            this.ID = ID;
            CreationTime = DateTime.Now;
        }
    }
}