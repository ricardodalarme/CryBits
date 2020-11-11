using CryBits.Server.Logic;
using CryBits.Entities;
using System;
using System.Collections.Generic;
using static CryBits.Server.Logic.Utils;

namespace CryBits.Server.Entities
{
    [Serializable]
    class Item : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Item> List = new Dictionary<Guid, Item>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Item Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Geral
        public string Description = string.Empty;
        public short Texture;
        public byte Type;
        public bool Stackable;
        public byte Bind;
        public byte Rarity;

        // Requerimentos
        public short Req_Level;
        private Guid req_Class;
        public Class Req_Class
        {
            get => Class.Get(req_Class);
            set => req_Class = new Guid(value.GetID());
        }

        // Poção
        public int Potion_Experience;
        public short[] Potion_Vital = new short[(byte)Vitals.Count];

        // Equipamento
        public byte Equip_Type;
        public short[] Equip_Attribute = new short[(byte)Attributes.Count];
        public short Weapon_Damage;

        // Construtor
        public Item(Guid ID) : base(ID) { }
    }
}
