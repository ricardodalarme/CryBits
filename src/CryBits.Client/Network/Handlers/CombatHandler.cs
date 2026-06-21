using CryBits.Client.Components;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Network.Handlers;

internal class CombatHandler(GameContext context)
{
    [PacketHandler]
    internal void Attack(CombatAttackPacket packet)
    {
        var attackerId = packet.AttackerId;
        var victimId = packet.VictimId;
        var attacker = context.GetNetworkEntity(attackerId);
        if (attacker is null) return;

        var attack = context.World.Get<AttackComponent>(attacker.Value);
        if (attack is null) return;
        attack.AttackCountdown = AttackSpeed / 1000f;

        if (victimId is null) return;

        var victim = context.GetNetworkEntity(victimId.Value);
        if (victim is null) return;
        var world = context.World;
        var victimMovement = world.Get<MovementComponent>(victim.Value);
        if (victimMovement is null) return;
        BloodSplatSpawner.Spawn(world, victimMovement.TileX, victimMovement.TileY);
        var hurt = world.AddOrGet<HurtComponent>(victim.Value);
        if (hurt is null) return;
        hurt.HurtCountdown = HurtComponent.Duration;
    }
}
