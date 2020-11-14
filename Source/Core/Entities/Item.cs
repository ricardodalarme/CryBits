using System;
using System.Collections.Generic;

namespace CryBits.Entities
{
    [Serializable]
    public class Item : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Item> List = new Dictionary<Guid, Item>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Item Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Geral
        public string Description { get; set; } = string.Empty;
        public short Texture { get; set; }
        public Items Type { get; set; }
        public bool Stackable { get; set; }
        public BindOn Bind { get; set; }
        public Rarity Rarity { get; set; }

        // Requerimentos
        public short ReqLevel;
        private Guid _reqClass;
        public Class ReqClass
        {
            get => Class.Get(_reqClass);
            set => _reqClass = new Guid(value.GetID());
        }

        // Poção
        public int PotionExperience { get; set; }
        public short[] PotionVital { get; set; } = new short[(byte)Vitals.Count];

        // Equipamento
        public byte EquipType { get; set; }
        public short[] EquipAttribute { get; set; } = new short[(byte)Attributes.Count];
        public short WeaponDamage { get; set; }
    }
}
