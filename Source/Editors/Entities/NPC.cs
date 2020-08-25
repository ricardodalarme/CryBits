using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Logic.Utils;

namespace Entities
{
    class NPC : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, NPC> List = new Dictionary<Guid, NPC>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static NPC Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        public string Name = string.Empty;
        public string SayMsg = string.Empty;
        public short Texture;
        public byte Behaviour;
        public byte SpawnTime;
        public byte Sight;
        public int Experience;
        public short[] Vital = new short[(byte)Vitals.Count];
        public short[] Attribute = new short[(byte)Attributes.Count];
        public BindingList<NPC_Drop> Drop = new BindingList<NPC_Drop>();
        public bool AttackNPC;
        public BindingList<NPC> Allie = new BindingList<NPC>();
        public NPC_Movements Movement;
        public byte Flee_Helth;
        public Shop Shop;

        public NPC(Guid ID) : base(ID) { }
        public override string ToString() => Name;
    }

    class NPC_Drop : Lists.Structures.Inventory
    {
        public byte Chance;

        public NPC_Drop(Item Item, short Amount, byte Chance) : base(Item, Amount)
        {
            this.Chance = Chance;
        }
        public override string ToString() => Item.Name + " [" + Amount + "x, " + Chance + "%]";
    }
}