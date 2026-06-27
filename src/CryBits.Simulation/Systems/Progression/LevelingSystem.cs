using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Simulation.Formulas;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using Attribute = CryBits.Definitions.Characters.Attribute;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Progression;

public sealed class LevelingSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is AddPointIntent add)
                AddPoint(world, add.SourceEntityId, add.AttributeNum);
        }

        foreach (var ev in tick.Events.Events)
        {
            if (ev is XpAwardedEvent xpEvent)
            {
                var playerId = world.FindPlayer(xpEvent.EntityId);
                if (playerId != null)
                    GiveExperience(world, playerId.Value, xpEvent.Amount);
            }

            if (ev is PlayerDiedEvent playerDied)
            {
                if (playerDied.SourceId.HasValue)
                {
                    var killerId = world.FindPlayer(playerDied.SourceId.Value);
                    if (killerId != null)
                    {
                        var victimId = world.FindPlayer(playerDied.EntityId);
                        if (victimId != null)
                        {
                            var victimLevel = world.Get<LevelComponent>(victimId.Value);
                            if (victimLevel != null && victimLevel.Experience / 10 > 0)
                                GiveExperience(world, killerId.Value, victimLevel.Experience / 10);
                        }
                    }
                }

                var victimId2 = world.FindPlayer(playerDied.EntityId);
                if (victimId2 != null)
                {
                    var victimLevel = world.Get<LevelComponent>(victimId2.Value);
                    if (victimLevel == null) continue;
                    victimLevel.Experience /= 10;
                    world.MarkDirty<LevelComponent>(victimId2.Value);
                }
            }

            if (ev is NpcDiedEvent npcDied && npcDied.SourceId.HasValue)
            {
                var killerId = world.FindPlayer(npcDied.SourceId.Value);
                if (killerId == null) continue;

                var xp = catalog.Npcs.Get(npcDied.NpcDefId)?.Experience ?? 0;
                if (xp > 0)
                    GiveExperience(world, killerId.Value, xp);
            }
        }
    }

    private void AddPoint(World world, EntityId entityId, byte attributeNum)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var level = e.Get<LevelComponent>()!;
        var attrs = e.Get<AttributesComponent>()!;

        if (level.Points <= 0) return;

        attrs.Values[attributeNum]++;
        level.Points--;
        world.MarkDirty<LevelComponent>(entityId);
        world.MarkDirty<AttributesComponent>(entityId);
    }

    private void GiveExperience(World world, EntityId entityId, int value)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var level = e.Get<LevelComponent>()!;
        var party = e.Get<PartyState>();

        if (party?.Members.Count > 0 && value > 0)
            PartySplitXp(world, entityId, value);
        else
            level.Experience += value;

        if (level.Experience < 0) level.Experience = 0;

        CheckLevelUp(world, entityId);
    }

    private void CheckLevelUp(World world, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var level = e.Get<LevelComponent>()!;
        var attrs = e.Get<AttributesComponent>()!;

        byte numLevel = 0;

        short totalAttr = 0;
        for (byte i = 0; i < (byte)Attribute.Count; i++) totalAttr += attrs.Values[i];
        var expNeeded = LevelingFormulas.ExperienceNeeded(level.Level, totalAttr, (byte)level.Points);

        while (level.Experience >= expNeeded)
        {
            numLevel++;
            var expRest = level.Experience - expNeeded;

            level.Level++;
            level.Points += Config.NumPoints;
            level.Experience = expRest;

            totalAttr = 0;
            for (byte i = 0; i < (byte)Attribute.Count; i++) totalAttr += attrs.Values[i];
            expNeeded = LevelingFormulas.ExperienceNeeded(level.Level, totalAttr, (byte)level.Points);
        }

        world.MarkDirty<LevelComponent>(entityId);
        world.MarkDirty<AttributesComponent>(entityId);
    }

    private void PartySplitXp(World world, EntityId entityId, int value)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var level = e.Get<LevelComponent>()!;
        var party = e.Get<PartyState>();
        if (party == null) return;

        var diff = new double[party.Members.Count];
        double diffSum = 0;

        for (byte i = 0; i < party.Members.Count; i++)
        {
            var memberE = world.Entities.Get(party.Members[i]);
            if (memberE == null) continue;
            var memberLevel = memberE.Get<LevelComponent>()!;
            var difference = Math.Abs(level.Level - memberLevel.Level);
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
            world.MarkDirty<LevelComponent>(party.Members[i]);
        }

        level.Experience += value - experienceSum;
        CheckLevelUp(world, entityId);
        world.MarkDirty<LevelComponent>(entityId);
    }
}
