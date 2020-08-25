using System;
using System.Collections.Generic;
using static Logic.Utils;

namespace Entities
{
    [Serializable]
    class NPC : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, NPC> List = new Dictionary<Guid, NPC>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static NPC Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        public string Name;
        public string SayMsg;
        public short Texture;
        public byte Behaviour;
        public byte SpawnTime;
        public byte Sight;
        public int Experience;
        public short[] Vital = new short[(byte)Vitals.Count];
        public short[] Attribute = new short[(byte)Attributes.Count];
        public NPC_Drop[] Drop;
        public bool AttackNPC;
        public NPC[] Allie;
        public NPC_Movements Movement;
        public byte Flee_Helth;
        private Guid shop;

        public Shop Shop
        {
            get => Shop.Get(shop);
            set => shop = new Guid(Entity.GetID(value));
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
    class NPC_Drop
    {
        // Dados
        private Guid item;
        public Item Item
        {
            get => Item.Get(item);
            set => item = new Guid(Entity.GetID(value));
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