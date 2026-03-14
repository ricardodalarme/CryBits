using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Movement;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Client.Network.Handlers;

internal class CombatHandler(GameContext context)
{
    [PacketHandler]
    internal void Attack(CombatAttackPacket packet)
    {
        var attackerId = packet.AttackerId;
        var victimId = packet.VictimId;
        var attacker = context.GetNetworkEntity(attackerId);

        ref var state = ref context.World.Get<AttackComponent>(attacker);
        state.AttackCountdown = AttackSpeed / 1000f;

        if (victimId is null) return;

        var victim = context.GetNetworkEntity(victimId.Value);
        var world = context.World;
        ref var victimMovement = ref world.Get<MovementComponent>(victim);
        BloodSplatSpawner.Spawn(world, victimMovement.TileX, victimMovement.TileY);
        ref var tint = ref world.Get<DamageComponent>(victim);
        tint.HurtCountdown = DamageComponent.Duration;
    }
}
