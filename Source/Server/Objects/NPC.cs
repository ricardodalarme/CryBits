using System;

namespace Objects
{
    [Serializable]
    class NPC : Lists.Structures.Data
    {
        public string Name;
        public string SayMsg;
        public short Texture;
        public byte Behaviour;
        public byte SpawnTime;
        public byte Sight;
        public int Experience;
        public short[] Vital = new short[(byte)Game.Vitals.Count];
        public short[] Attribute = new short[(byte)Game.Attributes.Count];
        public NPC_Drop[] Drop;
        public bool AttackNPC;
        public NPC[] Allie;
        public global::NPC.Movements Movement;
        public byte Flee_Helth;
        private Guid shop;

        public Shop Shop
        {
            get => (Shop)Lists.GetData(Lists.Shop, shop);
            set => shop = new Guid(Lists.GetID(value));
        }

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
            get => (Item)Lists.GetData(Lists.Item, item);
            set => item = new Guid(Lists.GetID(value));
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