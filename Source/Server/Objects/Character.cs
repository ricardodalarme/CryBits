﻿namespace Objects
{
    class Character
    {
        // Dados básicos de todos personagens
        public TMap Map;
        public byte X;
        public byte Y;
        public Utils.Directions Direction;
        public short[] Vital = new short[(byte)Utils.Vitals.Count];
    }
}