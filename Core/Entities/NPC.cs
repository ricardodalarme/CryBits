using System;
using System.Collections.Generic;

namespace CryBits.Entities
{
    [Serializable]
    public class NPC : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, NPC> List = new Dictionary<Guid, NPC>();

        // Dados
        public string SayMsg { get; set; }
        public short Texture { get; set; }
        public NPCs Behaviour { get; set; }
        public byte SpawnTime { get; set; }
        public byte Sight { get; set; }
        public int Experience { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Vitals.Count];
        public short[] Attribute { get; set; } = new short[(byte)Attributes.Count];
        public IList<NPCDrop> Drop { get; set; } = new List<NPCDrop>();
        public bool AttackNPC { get; set; }
        public IList<NPC> Allie { get; set; } = new List<NPC>();
        public NPCMovements Movement { get; set; }
        public byte FleeHealth { get; set; }
        private Guid _shop;

        public Shop Shop
        {
            get => Shop.List.Get(_shop);
            set => _shop = value.GetID();
        }

        public bool IsAllied(NPC npc)
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
            Chance = chance;
        }
    }
}