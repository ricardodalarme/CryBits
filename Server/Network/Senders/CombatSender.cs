using CryBits.Packets.Server;
using CryBits.Server.Entities;
using System;

namespace CryBits.Server.Network.Senders;

internal sealed class CombatSender(PackageSender packageSender)
{
    public static CombatSender Instance { get; } = new(PackageSender.Instance);

    public void Attack(MapInstance map, Guid attackerId, Guid? victimId = null)
    {
        packageSender.ToMap(map, new CombatAttackPacket { AttackerId = attackerId, VictimId = victimId });
    }
}
