using System;

namespace Objects
{
    class NPC : Lists.Structures.Data
    {
        public string Name;
        public string SayMsg;
        public short Texture;
        public byte Type;
        public short[] Vital = new short[(byte)Game.Vitals.Count];

        public NPC(Guid ID) : base(ID) { }
    }
}
