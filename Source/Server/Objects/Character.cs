namespace Objects
{
    class Character
    {
        // Dados
        public TMap Map;
        public byte X;
        public byte Y;
        public Game.Directions Direction;
        public short[] Vital = new short[(byte)Game.Vitals.Count];
    }
}