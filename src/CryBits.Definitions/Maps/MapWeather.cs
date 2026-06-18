using MemoryPack;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public partial class MapWeather
{
    public Weather Type { get; set; }
    public byte Intensity { get; set; }
}
