using System;
using System.Collections.Generic;

namespace CryBits.Entities
{
    [Serializable]
    public class Class : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Class> List = new Dictionary<Guid, Class>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Class Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Dados
        public string Description { get; set; }
        public IList<short> Tex_Male { get; set; } = new List<short>();
        public IList<short> Tex_Female { get; set; } = new List<short>();
        public Map Spawn_Map { get; set; }
        public byte Spawn_Direction { get; set; }
        public byte Spawn_X { get; set; }
        public byte Spawn_Y { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Vitals.Count];
        public short[] Attribute { get; set; } = new short[(byte)Attributes.Count];
        public IList<ItemSlot> Item { get; set; } = new List<ItemSlot>();

        // Construtor
        public Class(Guid id) : base(id) { }
    }
}
