using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Simulation.Formulas;
using System;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using Attribute = CryBits.Definitions.Characters.Attribute;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Systems.Progression;

internal sealed class LevelingSystem : ISimulationSystem
{
    public static LevelingSystem Instance { get; } = new();

    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is AddPointIntent add)
                AddPoint(world, add.SourceEntityId, add.AttributeNum);
        }

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
                GiveExperience(world, killerId.Value, xp);
        }
    }

    private void AddPoint(World world, EntityId entityId, byte attributeNum)
    {
        var e = world.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;

        if (stats.Points <= 0) return;

        stats.Attribute[attributeNum]++;
        stats.Points--;
        world.Dirty.Mark<StatBlock>(entityId);
    }

    internal void GiveExperience(World world, EntityId entityId, int value)
    {
        var e = world.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;
        var party = e.Get<PartyState>();

        if (party?.Members.Count > 0 && value > 0)
            PartySplitXp(world, entityId, value);
        else
            stats.Experience += value;

        if (stats.Experience < 0) stats.Experience = 0;

        CheckLevelUp(world, entityId);
    }

    private void CheckLevelUp(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId)!;
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

        world.Dirty.Mark<StatBlock>(entityId);
    }

    private void PartySplitXp(World world, EntityId entityId, int value)
    {
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

            GiveExperience(world, party.Members[i], givenExperience);
            world.Dirty.Mark<StatBlock>(party.Members[i]);
        }

        stats.Experience += value - experienceSum;
        CheckLevelUp(world, entityId);
        world.Dirty.Mark<StatBlock>(entityId);
    }
}
