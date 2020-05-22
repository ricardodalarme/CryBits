using System;

namespace Objects
{
    [Serializable]
    class Class : Lists.Structures.Data
    {
        public string Name;
        public string Description;
        public short[] Tex_Male = Array.Empty<short>();
        public short[] Tex_Female = Array.Empty<short>();
        public Map Spawn_Map;
        public byte Spawn_Direction;
        public byte Spawn_X;
        public byte Spawn_Y;
        public short[] Vital = new short[(byte)Game.Vitals.Count];
        public short[] Attribute = new short[(byte)Game.Attributes.Count];
        public Tuple<Item, short>[] Item = Array.Empty<Tuple<Item, short>>();

        public Class(Guid ID) : base(ID) { }
    }
}
