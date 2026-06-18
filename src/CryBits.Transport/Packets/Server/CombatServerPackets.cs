using MemoryPack;

namespace CryBits.Transport.Packets.Server;

[MemoryPackable]
public partial class CombatAttackPacket : IServerPacket
{
    public long AttackerId;
    public long? VictimId;
}
