using static Logic.Utils;

namespace Objects
{
    class Character
    {
        // Dados básicos de todos personagens
        public TMap Map;
        public byte X;
        public byte Y;
        public Directions Direction;
        public short[] Vital = new short[(byte)Vitals.Count];
    }
}