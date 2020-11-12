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
        public static NPC Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Dados
        public string SayMsg { get; set; }
        public short Texture { get; set; }
        public NPCBehaviour Behaviour { get; set; }
        public byte SpawnTime { get; set; }
        public byte Sight { get; set; }
        public int Experience { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Vitals.Count];
        public short[] Attribute { get; set; } = new short[(byte)Attributes.Count];
        public IList<NPCDrop> Drop { get; set; } = new List<NPCDrop>();
        public bool AttackNPC { get; set; }
        public IList<NPC> Allie { get; set; } = new List<NPC>();
        public NPCMovements Movement { get; set; }
        public byte Flee_Helth { get; set; }
        private Guid _shop;

        public Shop Shop
        {
            get => Shop.Get(_shop);
            set => _shop = new Guid(value.GetID());
        }

        // Construtor
        public NPC(Guid id) : base(id) { }

        public bool IsAlied(NPC npc)
        {
            // Verifica se o NPC é aliado do outro
            for (byte i = 0; i < Allie.Count; i++)
                if (Allie[i] == npc)
                    return true;

            return false;
        }
    }

    [Serializable]
    public class NPCDrop : ItemSlot
    {
        public byte Chance;

        public NPCDrop(Item item, short amount, byte chance) : base(item, amount)
        {
            this.Chance = chance;
        }
    }
}