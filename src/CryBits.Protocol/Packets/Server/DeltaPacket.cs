using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class DeltaPacket : IServerPacket
{
    public long TickNumber;
    public long BaselineTick;
    public Guid MapId;
    public List<EntityDelta> Entities = [];
    public List<long> RemovedEntities = [];
}

[MemoryPackable]
public partial class EntityDelta
{
    public long EntityId;
    public EntityKind Kind;
    public DeltaAction Action;
    public List<ComponentData> Components = [];
    public List<byte> RemovedTags = [];
}

public enum DeltaAction : byte
{
    Added = 0,
    Changed = 1
}
