using CryBits.Definitions.Maps;
using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class ChunkPayload : IServerPacket
{
    public Guid MapId;
    public short ChunkX;
    public short ChunkY;
    public long Version;
    public byte[] TileData = [];
    public WeatherType? WeatherOverride;
    public FogConfig? FogOverride;
    public byte? LightingOverride;
}

[MemoryPackable]
public partial class ChunkRevisionPacket : IServerPacket
{
    public Guid MapId;
    public short ChunkX;
    public short ChunkY;
    public long Version;
}
