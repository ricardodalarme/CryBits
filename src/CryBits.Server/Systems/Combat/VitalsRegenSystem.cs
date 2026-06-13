using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Server.World;
using CryBits.Simulation.Core;
using CryBits.Simulation.Formulas;
using System;
using System.Linq;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Server.Systems.Combat;

internal sealed class VitalsRegenSystem : ISimulationSystem
{
    public static VitalsRegenSystem Instance { get; } = new();

    private long _lastRegenTick;

    public void Execute(GameWorld world, Tick tick)
    {
        if (Environment.TickCount64 <= _lastRegenTick + 5000) return;
        _lastRegenTick = Environment.TickCount64;

        foreach (var session in world.Sessions.Where(a => a.IsPlaying))
        {
            if (session.Character is not { } playerId) continue;
            var e = world.Entities.Get(playerId)!;
            var vitals = e.Get<Vitals>()!;
            var stats = e.Get<StatBlock>()!;

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

                world.Dirty.Mark<Vitals>(playerId);
            }
        }

        foreach (var map in world.Maps.Values)
        {
            foreach (var npcId in map.NpcIds)
            {
                var e = world.Entities.Get(npcId);
                if (e == null) continue;
                var npcState = e.Get<NpcState>();
                if (npcState == null || !npcState.Alive) continue;
                var vitals = e.Get<Vitals>()!;
                var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);

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
