using CryBits.Entities;
using System;
using System.Collections.Generic;

namespace CryBits.Client.Entities
{
    class Item : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Item> List;

        // Obtém o dado, caso ele não existir retorna nulo
        public static Item Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Geral
        public string Description = string.Empty;
        public short Texture;
        public byte Type;
        public byte Rarity;
        public BindOn Bind;
        // Requerimentos
        public short Req_Level;
        public Class Req_Class;
        // Poção
        public int Potion_Experience;
        public short[] Potion_Vital = new short[(byte)Vitals.Count];
        // Equipamento
        public byte Equip_Type;
        public short[] Equip_Attribute = new short[(byte)Equipments.Count];
        public short Weapon_Damage;

        public Item(Guid id) : base(id) { }
    }
}
