using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Formulas;
using System;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Simulation.Systems.Combat;

public sealed class VitalsRegenSystem(DefinitionCatalog catalog) : ISimulationSystem
{


    private long _lastRegenTick;

    public void Execute(World world, Tick tick)
    {
        if (Environment.TickCount64 <= _lastRegenTick + 5000) return;
        _lastRegenTick = Environment.TickCount64;

        foreach (var state in world.Entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var vitals = state.Get<Vitals>()!;
            var stats = state.Get<StatBlock>()!;

            for (byte v = 0; v < (byte)Vital.Count; v++)
            {
                var current = v == 0 ? vitals.Hp : vitals.Mp;
                var max = v == 0 ? vitals.MaxHp : vitals.MaxMp;
                if (current >= max) continue;

                var regen = VitalFormulas.PlayerRegeneration(
                    (Vital)v,
                    max,
                    stats.Attribute[(byte)Attribute.Vitality],
                    stats.Attribute[(byte)Attribute.Intelligence]);
                current += regen;
                if (current > max) current = max;
                if (v == 0) vitals.Hp = current; else vitals.Mp = current;

                world.Dirty.Mark<Vitals>(state.Id);
            }
        }

        foreach (var map in world.Maps.Values)
        {
            foreach (var npcId in map.NpcIds)
            {
                var e = world.Entities.Get(npcId);
                if (e == null) continue;
                var npcState = e.Get<NpcState>();
                if (npcState == null) continue;
                var vitals = e.Get<Vitals>()!;
                var npcData = catalog.Npcs.Get(npcState.NpcDefId);

                for (byte v = 0; v < (byte)Vital.Count; v++)
                {
                    var current = v == 0 ? vitals.Hp : vitals.Mp;
                    var max = npcData.Vital[v];
                    if (current >= max) continue;

                    var regen = VitalFormulas.NpcRegeneration(
                        (Vital)v,
                        npcData.Vital[v],
                        npcData.Attribute[(byte)Attribute.Vitality],
                        npcData.Attribute[(byte)Attribute.Intelligence]);
                    current += regen;
                    if (current > max) current = max;
                    if (v == 0) vitals.Hp = current; else vitals.Mp = current;

                    world.Dirty.Mark<Vitals>(npcId);
                }
            }
        }
    }
}
