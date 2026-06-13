using CryBits.Network.Packets.Server;
using LiteNetLib;
using System;

namespace CryBits.Host.Network.Senders;

internal sealed class CombatSender(PackageSender packageSender)
{
    public static CombatSender Instance { get; } = new(PackageSender.Instance);

    public void Attack(Guid mapId, Guid attackerId, Guid? victimId = null)
    {
        packageSender.ToMap(mapId, new CombatAttackPacket { AttackerId = attackerId, VictimId = victimId },
            DeliveryMethod.ReliableUnordered);
    }
}
