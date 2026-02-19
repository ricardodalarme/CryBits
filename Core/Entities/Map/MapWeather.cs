using System;
using CryBits.Enums;

namespace CryBits.Entities.Map;

[Serializable]
public class MapWeather
{
    public Weather Type { get; set; }
    public byte Intensity { get; set; }
}
