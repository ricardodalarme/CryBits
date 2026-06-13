using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;

namespace CryBits.Simulation.Systems.Combat;

public sealed class AggroSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            if (ev is not CombatAttackEvent attack || !attack.Hit) continue;
            if (!attack.VictimId.HasValue) continue;

            var victimE = world.Entities.Get(attack.VictimId.Value);
            if (victimE == null) continue;

            var npcState = victimE.Get<NpcState>();
            if (npcState == null) continue;

            if (npcState.TargetId == null)
            {
                npcState.TargetId = attack.AttackerId;
                world.Dirty.Mark<NpcState>(attack.VictimId.Value);
            }
        }
    }
}
