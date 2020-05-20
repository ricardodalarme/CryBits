using System;

namespace Objects
{
    [Serializable]
    class NPC
    {
        public string Name;
        public string SayMsg;
        public short Texture;
        public byte Behaviour;
        public byte SpawnTime;
        public byte Sight;
        public int Experience;
        public short[] Vital;
        public short[] Attribute;
        public NPC_Drop[] Drop;
        public bool AttackNPC;
        public short[] Allie;
        public global::NPC.Movements Movement;
        public byte Flee_Helth;
        private Guid shop;

        public Shop Shop
        {
            get => (Shop)Lists.GetData(Lists.Shop, shop);
            set => shop = new Guid(Lists.GetID(value));
        }

        public bool IsAlied(short Index)
        {
            // Verifica se o NPC é aliado do outro
            for (byte i = 0; i < Allie.Length; i++)
                if (Allie[i] == Index)
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