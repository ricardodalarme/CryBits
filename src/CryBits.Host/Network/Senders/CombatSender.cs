using CryBits.Transport.Packets.Server;
using CryBits.Simulation.State;
using LiteNetLib;
using System;

namespace CryBits.Host.Network.Senders;

internal sealed class CombatSender(PackageSender packageSender)
{
    public static CombatSender Instance { get; } = new(PackageSender.Instance);

    public void Attack(Guid mapId, EntityId attackerId, EntityId? victimId = null)
    {
        packageSender.ToMap(mapId, new CombatAttackPacket { AttackerId = attackerId.Value, VictimId = victimId?.Value },
            DeliveryMethod.ReliableUnordered);
    }
}
