using CryBits.Server.Entities;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.Events;
using CryBits.Simulation.Formulas;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using System;
using static CryBits.Globals;

namespace CryBits.Server.Systems.Progression;

internal sealed class LevelingSystem(PlayerSender playerSender, MapSender mapSender) : ISimulationSystem
{
    public static LevelingSystem Instance { get; } = new(PlayerSender.Instance, MapSender.Instance);

    internal void AddPoint(Player player, byte attributeNum)
    {
        if (player.Points <= 0) return;

        player.Attribute[attributeNum]++;
        player.Points--;
        playerSender.PlayerExperience(player);
        mapSender.MapPlayers(player);
    }

    public void GiveExperience(Player player, int value)
    {
        if (player.Party.Count > 0 && value > 0)
            PartySplitXp(player, value);
        else
            player.Experience += value;

        if (player.Experience < 0) player.Experience = 0;

        CheckLevelUp(player);
    }

    private void CheckLevelUp(Player player)
    {
        byte numLevel = 0;

        while (player.Experience >= player.ExpNeeded)
        {
            numLevel++;
            var expRest = player.Experience - player.ExpNeeded;

            player.Level++;
            player.Points += Config.NumPoints;
            player.Experience = expRest;
        }

        playerSender.PlayerExperience(player);
        if (numLevel > 0) mapSender.MapPlayers(player);
    }

    private void PartySplitXp(Player player, int value)
    {
        var diff = new double[player.Party.Count];
        double diffSum = 0;

        for (byte i = 0; i < player.Party.Count; i++)
        {
            var difference = Math.Abs(player.Level - player.Party[i].Level);
            diff[i] = LevelingFormulas.PartyXpWeight(difference);
            diffSum += diff[i];
        }

        var experienceSum = 0;
        for (byte i = 0; i < player.Party.Count; i++)
        {
            if (diffSum > 1) diff[i] *= 1 / diffSum;

            var givenExperience = (int)(value / 2 * diff[i]);
            experienceSum += givenExperience;

            GiveExperience(player.Party[i], givenExperience);
            playerSender.PlayerExperience(player.Party[i]);
        }

        player.Experience += value - experienceSum;
        CheckLevelUp(player);
        playerSender.PlayerExperience(player);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            if (ev is EntityDiedEvent died && died.Source is Player killer)
            {
                var xp = died.Entity switch
                {
                    Player victim => victim.Experience / 10,
                    NpcInstance npc => npc.Data.Experience,
                    _ => 0
                };

                if (xp > 0)
                    GiveExperience(killer, xp);
            }
        }
    }
}
