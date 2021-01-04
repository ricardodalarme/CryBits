using CryBits.Enums;
using System;
using System.Collections.Generic;

namespace CryBits.Entities
{
    [Serializable]
    public class Npc : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Npc> List = new();

        // Dados
        public string SayMsg { get; set; }
        public short Texture { get; set; }
        public Behaviour Behaviour { get; set; }
        public byte SpawnTime { get; set; }
        public byte Sight { get; set; }
        public int Experience { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Enums.Vital.Count];
        public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
        public IList<NpcDrop> Drop { get; set; } = new List<NpcDrop>();
        public bool AttackNpc { get; set; }
        public IList<Npc> Allie { get; set; } = new List<Npc>();
        public MovementStyle Movement { get; set; }
        public byte FleeHealth { get; set; }
        private Guid _shop;

        public Shop Shop
        {
            get => Shop.List.Get(_shop);
            set => _shop = value.GetID();
        }

        public bool IsAllied(Npc Npc)
        {
            // Verifica se o Npc é aliado do outro
            for (byte i = 0; i < Allie.Count; i++)
                if (Allie[i] == Npc)
                    return true;

            return false;
        }
    }

    [Serializable]
    public class NpcDrop : ItemSlot
    {
        public byte Chance;

        public NpcDrop(Item item, short amount, byte chance) : base(item, amount)
        {
            Chance = chance;
        }
    }
}