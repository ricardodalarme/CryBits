using System;

namespace CryBits.Network.Packets.Server;

[Serializable]
public struct CombatAttackPacket : IServerPacket
{
    public long AttackerId;
    public long? VictimId;
}
