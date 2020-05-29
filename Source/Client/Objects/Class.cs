using System;

namespace Objects
{
    class Class : Lists.Structures.Data
    {
        public string Name;
        public string Description;
        public short[] Tex_Male;
        public short[] Tex_Female;

        public Class(Guid ID) : base(ID) { }
    }
}
