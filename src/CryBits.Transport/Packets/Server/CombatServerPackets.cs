using System;

namespace CryBits.Transport.Packets.Server;

[Serializable]
public struct CombatAttackPacket : IServerPacket
{
    public long AttackerId;
    public long? VictimId;
}
