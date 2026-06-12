using System;

namespace CryBits.Definitions.Maps;

[Serializable]
public class MapFog
{
    public byte Texture { get; set; }
    public sbyte SpeedX { get; set; }
    public sbyte SpeedY { get; set; }
    public byte Alpha { get; set; } = 255;
}
