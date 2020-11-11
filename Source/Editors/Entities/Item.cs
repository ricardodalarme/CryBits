using CryBits;
using System;
using System.Collections.Generic;

namespace CryBits.Editors.Entities
{
    class Item : Entity
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
        public Class Req_Class;
        // Poção
        public int Potion_Experience;
        public short[] Potion_Vital = new short[(byte)Vitals.Count];
        // Equipamento
        public byte Equip_Type;
        public short[] Equip_Attribute = new short[(byte)Attributes.Count];
        public short Weapon_Damage;

        public Item(Guid ID) : base(ID) { }
        public override string ToString() => Name;
    }
}
