using System;

namespace CryBits.Network.Packets.Server;

[Serializable]
public struct CombatAttackPacket : IServerPacket
{
    public Guid AttackerId;
    public Guid? VictimId;
}
