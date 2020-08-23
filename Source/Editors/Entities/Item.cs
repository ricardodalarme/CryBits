using System;
using static Utils;

namespace Objects
{
    class Item : Data
    {
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
