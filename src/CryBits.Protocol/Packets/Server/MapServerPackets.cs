using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable] public partial class JoinMapPacket : IServerPacket;

[MemoryPackable]
public partial class MapRevisionPacket : IServerPacket
{
    public Guid MapId;
    public short Revision;
}

[MemoryPackable]
public partial class MapItemsPacket : IServerPacket
{
    public PacketsMapItem[] Items = [];
}

[MemoryPackable]
public partial struct PacketsMapItem
{
    public Guid ItemId;
    public byte X, Y;
}
