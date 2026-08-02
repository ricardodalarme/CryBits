using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Formulas;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;

namespace CryBits.Simulation.Systems.Progression;

public sealed class LevelingSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
            if (intent is AddPointIntent add)
            {
                AddPoint(world, add.SourceEntityId, add.AttributeNum);
            }
            else if (intent is XpShareIntent share)
            {
                if (share.Recipients == null || share.Recipients.Count == 0)
                    world.Remove<XpShareComponent>(share.SourceEntityId);
                else
                    world.Set(share.SourceEntityId, new XpShareComponent(share.Recipients));
            }

        foreach (var ev in tick.Events.Events)
        {
            if (ev is XpAwardedEvent xpEvent)
                if (world.Has<PlayerTag>(xpEvent.EntityId))
                    GiveExperience(world, xpEvent.EntityId, xpEvent.Amount);

            if (ev is PlayerDiedEvent playerDied)
            {
                if (playerDied.SourceId.HasValue)
                    if (world.Has<PlayerTag>(playerDied.SourceId.Value))
                        if (world.Has<PlayerTag>(playerDied.EntityId))
                        {
                            var victimLevel = world.Get<LevelComponent>(playerDied.EntityId);
                            if (victimLevel != null && victimLevel.Experience / 10 > 0)
                                GiveExperience(world, playerDied.SourceId.Value, victimLevel.Experience / 10);
                        }

                if (world.Has<PlayerTag>(playerDied.EntityId))
                {
                    var victimLevel = world.Get<LevelComponent>(playerDied.EntityId);
                    if (victimLevel == null) continue;
                    world.Update<LevelComponent>(playerDied.EntityId, l => l with { Experience = l.Experience / 10 });
                }
            }

            if (ev is NpcDiedEvent npcDied && npcDied.SourceId.HasValue)
            {
                if (!world.Has<PlayerTag>(npcDied.SourceId.Value)) continue;

                var xp = world.Catalog.Npcs.Get(npcDied.NpcDefId)?.Experience ?? 0;
                if (xp > 0)
                    GiveExperience(world, npcDied.SourceId.Value, xp);
            }
        }
    }

    private void AddPoint(World world, EntityId entityId, byte attributeNum)
    {
        if (!world.IsAlive(entityId)) return;
        var level = world.Get<LevelComponent>(entityId)!;
        var attrs = world.Get<AttributesComponent>(entityId)!;

        if (level.Points <= 0) return;

        var newValues = (short[])attrs.Values.Clone();
        newValues[attributeNum]++;
        world.Set(entityId, new AttributesComponent(newValues));
        world.Update<LevelComponent>(entityId, l => l with { Points = (short)(l.Points - 1) });
    }

    private void GiveExperience(World world, EntityId entityId, int value)
    {
        if (!world.IsAlive(entityId)) return;
        var xpShare = world.Get<XpShareComponent>(entityId);

        if (xpShare?.Recipients.Count > 0 && value > 0)
            PartySplitXp(world, entityId, value);
        else
            world.Update<LevelComponent>(entityId, l => l with { Experience = l.Experience + value });

        var currentLevel = world.Get<LevelComponent>(entityId);
        if (currentLevel != null && currentLevel.Experience < 0)
            world.Update<LevelComponent>(entityId, l => l with { Experience = 0 });

        CheckLevelUp(world, entityId);
    }

    private void CheckLevelUp(World world, EntityId entityId)
    {
        if (!world.IsAlive(entityId)) return;
        var level = world.Get<LevelComponent>(entityId)!;

        var expNeeded = LevelingFormulas.ExperienceNeeded(level.Level);

        if (level.Experience < expNeeded) return;

        var newLevel = level.Level;
        var newPoints = level.Points;
        var newExp = level.Experience;

        while (newExp >= expNeeded)
        {
            newLevel++;
            newPoints += Config.NumPoints;
            newExp -= expNeeded;

            expNeeded = LevelingFormulas.ExperienceNeeded(newLevel);
        }

        world.Set(entityId, new LevelComponent(Level: newLevel, Experience: newExp, Points: newPoints));
    }

    private void PartySplitXp(World world, EntityId entityId, int value)
    {
        if (!world.IsAlive(entityId)) return;
        var level = world.Get<LevelComponent>(entityId)!;
        var xpShare = world.Get<XpShareComponent>(entityId);
        if (xpShare == null) return;

        var diff = new double[xpShare.Recipients.Count];
        double diffSum = 0;

        for (byte i = 0; i < xpShare.Recipients.Count; i++)
        {
            var memberId = xpShare.Recipients[i];
            if (!world.IsAlive(memberId)) continue;
            var memberLevel = world.Get<LevelComponent>(memberId)!;
            var difference = Math.Abs(level.Level - memberLevel.Level);
            diff[i] = LevelingFormulas.PartyXpWeight(difference);
            diffSum += diff[i];
        }

        var experienceSum = 0;
        for (byte i = 0; i < xpShare.Recipients.Count; i++)
        {
            if (diffSum > 1) diff[i] *= 1 / diffSum;

            var givenExperience = (int)(value / 2 * diff[i]);
            experienceSum += givenExperience;

            GiveExperience(world, xpShare.Recipients[i], givenExperience);
        }

        world.Update<LevelComponent>(entityId, l => l with { Experience = l.Experience + value - experienceSum });
        CheckLevelUp(world, entityId);
    }
}
