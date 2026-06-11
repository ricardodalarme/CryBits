using System;

namespace CryBits.Packets.Server;

[Serializable]
public struct CombatAttackPacket : IServerPacket
{
    public Guid AttackerId;
    public Guid? VictimId;
}
