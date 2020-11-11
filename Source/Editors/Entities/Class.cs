using CryBits.Editors.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CryBits.Editors.Entities
{
    class Class : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Class> List = new Dictionary<Guid, Class>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Class Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        public string Name = string.Empty;
        public string Description;
        public BindingList<short> Tex_Male = new BindingList<short>();
        public BindingList<short> Tex_Female = new BindingList<short>();
        public Map Spawn_Map;
        public byte Spawn_Direction;
        public byte Spawn_X;
        public byte Spawn_Y;
        public short[] Vital = new short[(byte)Vitals.Count];
        public short[] Attribute = new short[(byte)Attributes.Count];
        public BindingList<Lists.Structures.Inventory> Item = new BindingList<Lists.Structures.Inventory>();

        public Class(Guid ID) : base(ID) { }

        public override string ToString() => Name;
    }
}