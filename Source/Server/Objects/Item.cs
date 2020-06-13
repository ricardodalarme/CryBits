﻿using System;
using System.Collections.Generic;
using static Utils;

namespace Objects
{
    [Serializable]
    class Item : Data
    {
        // Lista de dados
        public static Dictionary<Guid, Item> List = new Dictionary<Guid, Item>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Item Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Geral
        public string Name = string.Empty;
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
            get => Class.Get( req_Class);
            set => req_Class = new Guid(GetID(value));
        }
        // Poção
        public int Potion_Experience;
        public short[] Potion_Vital = new short[(byte)Utils.Vitals.Count];
        // Equipamento
        public byte Equip_Type;
        public short[] Equip_Attribute = new short[(byte)Utils.Attributes.Count];
        public short Weapon_Damage;

        // Construtor
        public Item(Guid ID) : base(ID) { }
    }
}
