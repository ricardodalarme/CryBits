namespace CryBits.Server.Entities
{
    internal abstract class Character
    {
        // Dados básicos de todos personagens
        public TempMap Map { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public Directions Direction { get; set; }
        public short[] Vital { get; set; } = new short[(byte)Vitals.Count];
    }
}