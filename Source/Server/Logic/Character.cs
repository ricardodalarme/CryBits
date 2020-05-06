class Character
{
    // Dados
    public short Map_Num;
    public byte X;
    public byte Y;
    public Game.Directions Direction;
    public short[] Vital = new short[(byte)Game.Vitals.Count];
}