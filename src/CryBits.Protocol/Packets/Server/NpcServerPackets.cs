using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class MapNpcsPacket : IServerPacket
{
    public PacketsMapNpc[] Npcs = [];
}

[MemoryPackable]
public partial class MapNpcPacket : IServerPacket
{
    public long InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital = [];
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
    public short[] Vital = [];
}

[MemoryPackable]
public partial class MapNpcDiedPacket : IServerPacket
{
    public long InstanceId;
}

[MemoryPackable]
public partial struct PacketsMapNpc
{
    public long InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital = [];

    public PacketsMapNpc()
    {
    }
}
