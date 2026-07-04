using CryBits.Client.Components;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Simulation.Components;
using CryBits.Simulation.State;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Combat;

internal sealed class AttackHitSystem(GameContext context) : IClientSystem
{
    public void Update(float dt)
    {
        var hits = new List<(EntityId, AttackHit)>();
        foreach (var state in context.World.All)
        {
            var hit = state.Get<AttackHit>();
            if (hit != null)
                hits.Add((state.Id, hit));
        }

        foreach (var (id, hit) in hits)
        {
            context.World.Set(id, new AttackComponent(AttackSpeed / 1000f));

            if (hit.VictimId != null)
            {
                var victimLocalId = context.GetNetworkEntity(hit.VictimId.Value);
                if (victimLocalId != null)
                {
                    var vicMov = context.World.Get<MovementComponent>(victimLocalId.Value);
                    if (vicMov != null)
                    {
                        BloodSplatSpawner.Spawn(context.World, vicMov.TileX, vicMov.TileY);
                        context.World.Set(victimLocalId.Value, new HurtComponent());
                    }
                }
                else
                {
                    BloodSplatSpawner.Spawn(context.World, hit.VictimTileX, hit.VictimTileY);
                }
            }

            var state = context.World.Entities.Get(id);
            state?.Remove<AttackHit>();
        }
    }
}
