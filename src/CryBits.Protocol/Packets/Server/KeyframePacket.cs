using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class KeyframePacket : IServerPacket
{
    public long TickNumber;
    public Guid MapId;
    public List<KeyframeEntity> Entities = [];
}

[MemoryPackable]
public partial class KeyframeEntity
{
    public long EntityId;
    public EntityKind Kind;
    public List<ComponentData> Components = [];
}

public enum EntityKind : byte
{
    Player = 0,
    Npc = 1,
    GroundItem = 2
}
