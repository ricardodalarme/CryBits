using CryBits.Definitions.Characters;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Formulas;
using static CryBits.Simulation.SimulationConstants;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Simulation.Systems.Regeneration;

public sealed class VitalsRegenSystem : ISimulationSystem
{
    private long _lastRegenTick;

    public void Execute(World world, Tick tick)
    {
        if (tick.TickNumber - _lastRegenTick < RegenIntervalTicks) return;
        _lastRegenTick = tick.TickNumber;

        foreach (var state in world.Entities.All)
        {
            if (!state.Has<Vitals>()) continue;

            var vitals = state.Get<Vitals>()!;
            var attrs = state.Get<AttributesComponent>();

            for (byte v = 0; v < (byte)Vital.Count; v++)
            {
                var current = v == 0 ? vitals.Hp : vitals.Mp;
                var max = v == 0 ? vitals.MaxHp : vitals.MaxMp;
                if (current >= max) continue;

                var vitality = attrs?.Values[(byte)Attribute.Vitality] ?? 0;
                var intelligence = attrs?.Values[(byte)Attribute.Intelligence] ?? 0;

                var regen = VitalFormulas.VitalRegeneration((Vital)v, max, vitality, intelligence);
                current += regen;
                if (current > max) current = max;
                if (v == 0) vitals.Hp = current; else vitals.Mp = current;

                world.MarkDirty<Vitals>(state.Id);
            }
        }
    }
}
