using CryBits;
using System;
using System.Collections.Generic;

namespace CryBits.Client.Entities
{
    class Item : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Item> List;

        // Obtém o dado, caso ele não existir retorna nulo
        public static Item Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Geral
        public string Name = string.Empty;
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

        public Item(Guid ID) : base(ID) { }
    }
}
