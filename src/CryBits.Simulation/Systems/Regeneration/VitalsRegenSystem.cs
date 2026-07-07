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

        foreach (var entity in world.Entities.All)
        {
            if (!world.Has<Vitals>(entity)) continue;

            var vitals = world.Get<Vitals>(entity)!;
            var attrs = world.Get<AttributesComponent>(entity);

            var newHp = vitals.Hp;
            var newMp = vitals.Mp;

            for (byte v = 0; v < (byte)Vital.Count; v++)
            {
                var current = v == 0 ? newHp : newMp;
                var max = v == 0 ? vitals.MaxHp : vitals.MaxMp;
                if (current >= max) continue;

                var vitality = attrs?.Values[(byte)Attribute.Vitality] ?? 0;
                var intelligence = attrs?.Values[(byte)Attribute.Intelligence] ?? 0;

                var regen = VitalFormulas.VitalRegeneration((Vital)v, max, vitality, intelligence);
                current += regen;
                if (current > max) current = max;
                if (v == 0) newHp = (short)current; else newMp = (short)current;
            }

            if (newHp != vitals.Hp || newMp != vitals.Mp)
                world.Set(entity, new Vitals(Hp: newHp, Mp: newMp, MaxHp: vitals.MaxHp, MaxMp: vitals.MaxMp));
        }
    }
}
