using System;

namespace Objects
{
    [Serializable]
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
        private Guid req_Class;
        public Class Req_Class
        {
            get => (Class)Lists.GetData(Lists.Class, req_Class);
            set => req_Class = new Guid(Lists.GetID(value));
        }
        // Poção
        public int Potion_Experience;
        public short[] Potion_Vital = new short[(byte)Game.Vitals.Count];
        // Equipamento
        public byte Equip_Type;
        public short[] Equip_Attribute = new short[(byte)Game.Attributes.Count];
        public short Weapon_Damage;

        // Construtor
        public Item(Guid ID) : base(ID) { }
    }
}
