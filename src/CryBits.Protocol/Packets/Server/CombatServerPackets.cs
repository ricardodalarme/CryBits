using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class CombatAttackPacket : IServerPacket
{
    public long AttackerId;
    public long? VictimId;
}
