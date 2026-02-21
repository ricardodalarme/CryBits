namespace CryBits.Client.Entities;

internal class MapBloodInstance(byte textureNum, short x, short y, byte opacity)
{
    public byte TextureNum = textureNum;
    public short X = x;
    public short Y = y;
    public byte Opacity = opacity;
}
