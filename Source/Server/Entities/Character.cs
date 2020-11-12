namespace CryBits.Server.Entities
{
    abstract class Character
    {
        // Dados básicos de todos personagens
        public TempMap Map;
        public byte X;
        public byte Y;
        public Directions Direction;
        public short[] Vital = new short[(byte)Vitals.Count];
    }
}