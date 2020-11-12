using System;
using System.Collections.Generic;

namespace CryBits.Entities
{
    [Serializable]
    public class NPC : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, NPC> List = new Dictionary<Guid, NPC>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static NPC Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        public string SayMsg { get; set; }
        public short Texture { get; set; }
        public NPCBehaviour Behaviour { get; set; }
        public byte SpawnTime { get; set; }
        public byte Sight { get; set; }
        public int Experience { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Vitals.Count];
        public short[] Attribute { get; set; } = new short[(byte)Attributes.Count];
        public IList<NPC_Drop> Drop { get; set; } = new List<NPC_Drop>();
        public bool AttackNPC { get; set; }
        public IList<NPC> Allie { get; set; } = new List<NPC>();
        public NPCMovements Movement { get; set; }
        public byte Flee_Helth { get; set; }
        private Guid shop;

        public Shop Shop
        {
            get => Shop.Get(shop);
            set => shop = new Guid(value.GetID());
        }

        // Construtor
        public NPC(Guid ID) : base(ID) { }

        public bool IsAlied(NPC NPC)
        {
            // Verifica se o NPC é aliado do outro
            for (byte i = 0; i < Allie.Count; i++)
                if (Allie[i] == NPC)
                    return true;

            return false;
        }
    }

    [Serializable]
    public class NPC_Drop : ItemSlot
    {
        public byte Chance;

        public NPC_Drop(Item Item, short Amount, byte Chance) : base(Item, Amount)
        {
            this.Chance = Chance;
        }
    }
}