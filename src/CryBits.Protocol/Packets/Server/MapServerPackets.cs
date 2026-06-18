using CryBits.Definitions.Maps;
using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class MapsPacket : IServerPacket
{
    public Dictionary<Guid, Map> List;
}

[MemoryPackable]
public partial class MapPacket : IServerPacket
{
    public Map Map;
}

[MemoryPackable] public partial class JoinMapPacket : IServerPacket;

[MemoryPackable]
public partial class MapRevisionPacket : IServerPacket
{
    public Guid MapId;
    public short Revision;
}

[MemoryPackable]
public partial class MapNpcsPacket : IServerPacket
{
    public PacketsMapNpc[] Npcs;
}

[MemoryPackable]
public partial class MapNpcPacket : IServerPacket
{
    public long InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}

[MemoryPackable]
public partial class MapNpcMovementPacket : IServerPacket
{
    public long InstanceId;
    public byte X, Y;
    public byte Direction;
    public byte Movement;
    public float Speed;
}

[MemoryPackable]
public partial class MapNpcDirectionPacket : IServerPacket
{
    public long InstanceId;
    public byte Direction;
}

[MemoryPackable]
public partial class MapNpcVitalsPacket : IServerPacket
{
    public long InstanceId;
    public short[] Vital;
}

[MemoryPackable]
public partial class MapNpcDiedPacket : IServerPacket
{
    public long InstanceId;
}

[MemoryPackable]
public partial class MapItemsPacket : IServerPacket
{
    public PacketsMapItem[] Items;
}

[MemoryPackable]
public partial struct PacketsMapItem
{
    public Guid ItemId;
    public byte X, Y;
}

[MemoryPackable]
public partial struct PacketsMapNpc
{
    public long InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}
