using System;
using System.ComponentModel;
using static Utils;

namespace Entities
{
    class Class : Entity
    {
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

        // Construtor
        public Class(Guid ID) : base(ID) { }
        public override string ToString() => Name;
    }
}