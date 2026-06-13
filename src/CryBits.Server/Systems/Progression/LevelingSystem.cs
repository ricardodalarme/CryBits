using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.State;
using CryBits.Server.Simulation.State.Components;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using CryBits.Simulation.Formulas;
using System;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Server.Systems.Progression;

internal sealed class LevelingSystem : ISimulationSystem
{
    public static LevelingSystem Instance { get; } = new();

    internal void AddPoint(EntityId entityId, byte attributeNum)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;

        if (stats.Points <= 0) return;

        stats.Attribute[attributeNum]++;
        stats.Points--;
        GameWorld.Current.Dirty.Mark<StatBlock>(entityId);
    }

    public void GiveExperience(EntityId entityId, int value)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;
        var party = e.Get<PartyState>();

        if (party?.Members.Count > 0 && value > 0)
            PartySplitXp(entityId, value);
        else
            stats.Experience += value;

        if (stats.Experience < 0) stats.Experience = 0;

        CheckLevelUp(entityId);
    }

    private void CheckLevelUp(EntityId entityId)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;

        byte numLevel = 0;

        short totalAttr = 0;
        for (byte i = 0; i < (byte)Attribute.Count; i++) totalAttr += stats.Attribute[i];
        var expNeeded = LevelingFormulas.ExperienceNeeded(stats.Level, totalAttr, stats.Points);

        while (stats.Experience >= expNeeded)
        {
            numLevel++;
            var expRest = stats.Experience - expNeeded;

            stats.Level++;
            stats.Points += Config.NumPoints;
            stats.Experience = expRest;

            totalAttr = 0;
            for (byte i = 0; i < (byte)Attribute.Count; i++) totalAttr += stats.Attribute[i];
            expNeeded = LevelingFormulas.ExperienceNeeded(stats.Level, totalAttr, stats.Points);
        }

        GameWorld.Current.Dirty.Mark<StatBlock>(entityId);
    }

    private void PartySplitXp(EntityId entityId, int value)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;
        var party = e.Get<PartyState>()!;

        var diff = new double[party.Members.Count];
        double diffSum = 0;

        for (byte i = 0; i < party.Members.Count; i++)
        {
            var memberE = world.Entities.Get(party.Members[i])!;
            var memberStats = memberE.Get<StatBlock>()!;
            var difference = Math.Abs(stats.Level - memberStats.Level);
            diff[i] = LevelingFormulas.PartyXpWeight(difference);
            diffSum += diff[i];
        }

        var experienceSum = 0;
        for (byte i = 0; i < party.Members.Count; i++)
        {
            if (diffSum > 1) diff[i] *= 1 / diffSum;

            var givenExperience = (int)(value / 2 * diff[i]);
            experienceSum += givenExperience;

            GiveExperience(party.Members[i], givenExperience);
            GameWorld.Current.Dirty.Mark<StatBlock>(party.Members[i]);
        }

        stats.Experience += value - experienceSum;
        CheckLevelUp(entityId);
        GameWorld.Current.Dirty.Mark<StatBlock>(entityId);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            if (ev is not EntityDiedEvent died) continue;
            if (!died.SourceId.HasValue || died.SourceIsPlayer != true) continue;

            var killerId = world.FindPlayerByValue(died.SourceId.Value);
            if (killerId == null) continue;

            var xp = died.EntityIsPlayer
                ? world.FindPlayerByValue(died.EntityId) is { } victimId
                    ? world.Entities.Get(victimId)!.Get<StatBlock>()!.Experience / 10
                    : 0
                : world.FindNpcInstance(died.EntityId) is { } npcId
                    ? DefinitionCatalog.Instance.Npcs.Get(world.Entities.Get(npcId)!.Get<NpcState>()!.NpcDefId)!.Experience
                    : 0;

            if (xp > 0)
                GiveExperience(killerId.Value, xp);
        }
    }
}
