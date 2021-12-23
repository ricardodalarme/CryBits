namespace CryBits.Client.Entities.TempMap;

internal class TempMapBlood
{
    // Dados
    public byte TextureNum;
    public short X;
    public short Y;
    public byte Opacity;

    // Construtor
    public TempMapBlood(byte textureNum, short x, short y, byte opacity)
    {
        TextureNum = textureNum;
        X = x;
        Y = y;
        Opacity = opacity;
    }
}