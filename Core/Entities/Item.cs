using System;
using System.Collections.Generic;
using CryBits.Enums;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Entities
{
    [Serializable]
    public class Item : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Item> List = new Dictionary<Guid, Item>();

        // Geral
        public string Description { get; set; } = string.Empty;
        public short Texture { get; set; }
        public Enums.ItemType Type { get; set; }
        public bool Stackable { get; set; }
        public BindOn Bind { get; set; }
        public Rarity Rarity { get; set; }

        // Requerimentos
        public short ReqLevel { get; set; }
        private Guid _reqClass;
        public Class ReqClass
        {
            get => Class.List.Get(_reqClass);
            set => _reqClass = value.GetID();
        }

        // Poção
        public int PotionExperience { get; set; }
        public short[] PotionVital { get; set; } = new short[(byte)Vital.Count];

        // Equipamento
        public byte EquipType { get; set; }
        public short[] EquipAttribute { get; set; } = new short[(byte)Attribute.Count];
        public short WeaponDamage { get; set; }
    }
}
