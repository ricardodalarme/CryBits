using CryBits.Enums;

namespace CryBits.Server.Entities
{
    internal abstract class Character
    {
        // Dados básicos de todos personagens
        public TempMap Map { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public Direction Direction { get; set; }
        public short[] Vital { get;  } = new short[(byte)Enums.Vital.Count];
    }
}