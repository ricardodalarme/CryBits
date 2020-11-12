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
        public byte Behaviour { get; set; }
        public byte SpawnTime { get; set; }
        public byte Sight { get; set; }
        public int Experience { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Vitals.Count];
        public short[] Attribute { get; set; } = new short[(byte)Attributes.Count];
        public NPC_Drop[] Drop { get; set; }
        public bool AttackNPC { get; set; }
        public NPC[] Allie { get; set; }
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
            for (byte i = 0; i < Allie.Length; i++)
                if (Allie[i] == NPC)
                    return true;

            return false;
        }
    }

    [Serializable]
    public class NPC_Drop
    {
        // Dados
        private Guid item;
        public Item Item
        {
            get => Item.Get(item);
            set => item = new Guid(value.GetID());
        }
        public short Amount;
        public byte Chance;

        // Construtor    
        public NPC_Drop(Item Item, short Amount, byte Chance)
        {
            this.Item = Item;
            this.Amount = Amount;
            this.Chance = Chance;
        }
    }
}