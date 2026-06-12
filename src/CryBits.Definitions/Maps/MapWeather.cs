using System;

namespace CryBits.Definitions.Maps;

[Serializable]
public class MapWeather
{
    public Weather Type { get; set; }
    public byte Intensity { get; set; }
}
