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
        public static Class Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        public string Description { get; set; }
        public short[] Tex_Male { get; set; } = Array.Empty<short>();
        public short[] Tex_Female { get; set; } = Array.Empty<short>();
        public Map Spawn_Map { get; set; }
        public byte Spawn_Direction { get; set; }
        public byte Spawn_X { get; set; }
        public byte Spawn_Y { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Vitals.Count];
        public short[] Attribute { get; set; } = new short[(byte)Attributes.Count];
        public Tuple<Item, short>[] Item { get; set; } = Array.Empty<Tuple<Item, short>>();

        // Construtor
        public Class(Guid ID) : base(ID) { }
    }
}
