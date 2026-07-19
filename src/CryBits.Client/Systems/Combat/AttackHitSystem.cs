using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Spawners;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Combat;

internal sealed class AttackHitSystem(GameContext context) : IClientSystem
{
    public void Update(World world, float dt)
    {
        var hits = new List<(EntityId, AttackHit)>();
        foreach (var entityId in world.All)
        {
            var hit = world.Get<AttackHit>(entityId);
            if (hit != null)
                hits.Add((entityId, hit));
        }

        foreach (var (id, hit) in hits)
        {
            world.Set(id, new AttackComponent(AttackSpeed / 1000f));

            if (hit.VictimId != null)
            {
                var victimLocalId = context.GetNetworkEntity(hit.VictimId.Value);
                if (victimLocalId != null)
                {
                    var vicMov = world.Get<MovementComponent>(victimLocalId.Value);
                    if (vicMov != null)
                    {
                        BloodSplatSpawner.Spawn(world, vicMov.TileX, vicMov.TileY);
                        world.Set(victimLocalId.Value, new HurtComponent());
                    }
                }
                else
                {
                    BloodSplatSpawner.Spawn(world, hit.VictimTileX, hit.VictimTileY);
                }
            }

            world.Remove<AttackHit>(id);
        }
    }
}
