﻿using System;
using System.Collections.Generic;

namespace CryBits.Entities
{
    [Serializable]
    public class Item : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Item> List = new Dictionary<Guid, Item>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Item Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Geral
        public string Description { get; set; } = string.Empty;
        public short Texture { get; set; }
        public Items Type { get; set; }
        public bool Stackable { get; set; }
        public BindOn Bind { get; set; }
        public Rarity Rarity { get; set; }

        // Requerimentos
        public short Req_Level;
        private Guid req_Class;
        public Class Req_Class
        {
            get => Class.Get(req_Class);
            set => req_Class = new Guid(value.GetID());
        }

        // Poção
        public int Potion_Experience { get; set; }
        public short[] Potion_Vital { get; set; } = new short[(byte)Vitals.Count];

        // Equipamento
        public byte Equip_Type { get; set; }
        public short[] Equip_Attribute { get; set; } = new short[(byte)Attributes.Count];
        public short Weapon_Damage { get; set; }

        // Construtor
        public Item(Guid ID) : base(ID) { }
    }
}