using CryBits.Definitions.Common;
using MemoryPack;
using System.Text.Json.Serialization;

namespace CryBits.Definitions.Maps;

[MemoryPackable]
public partial record class Map : Definition
{
    public Moral Moral { get; set; }
    public byte Panorama { get; set; }
    public string Music { get; set; } = string.Empty;
    public int ColorArgb { get; set; } = -1;

    public WeatherType DefaultWeather { get; set; }
    public FogConfig? DefaultFog { get; set; }
    public byte DefaultLighting { get; set; } = 100;

    [JsonIgnore]
    public Dictionary<ChunkCoord, MapChunk> Chunks { get; set; } = [];

    public IList<MapNpc> Npc { get; set; } = [];

    public Map()
    {
        Name = "New map";
    }
}
