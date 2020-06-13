using System;
using System.Collections.Generic;

namespace Objects
{
    [Serializable]
    class Class : Data
    {
        // Lista de dados
        public static Dictionary<Guid, Class> List = new Dictionary<Guid, Class>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Class Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        public string Name;
        public string Description;
        public short[] Tex_Male = Array.Empty<short>();
        public short[] Tex_Female = Array.Empty<short>();
        public Map Spawn_Map;
        public byte Spawn_Direction;
        public byte Spawn_X;
        public byte Spawn_Y;
        public short[] Vital = new short[(byte)Utils.Vitals.Count];
        public short[] Attribute = new short[(byte)Utils.Attributes.Count];
        public Tuple<Item, short>[] Item = Array.Empty<Tuple<Item, short>>();

        // Construtor
        public Class(Guid ID) : base(ID) { }
    }
}
