using System;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that manages player experience, leveling, and party XP distribution.
/// Also owns party-leave logic, keeping Player.cs free of party network calls.
/// </summary>
internal static class LevelingSystem
{
    /// <summary>Spends one attribute point for <paramref name="player"/> on the given attribute.</summary>
    internal static void AddPoint(Player player, byte attributeNum)
    {
        if (player.Points <= 0) return;

        player.Attribute[attributeNum]++;
        player.Points--;
        PlayerSender.PlayerExperience(player);
        MapSender.MapPlayers(player);
    }

    /// <summary>
    /// Grants <paramref name="value"/> experience to <paramref name="player"/>.
    /// If the player is in a party the XP is split across all members weighted by level
    /// difference; otherwise it is awarded directly.
    /// </summary>
    public static void GiveExperience(Player player, int value)
    {
        if (player.Party.Count > 0 && value > 0)
            PartySplitXp(player, value);
        else
            player.Experience += value;

        if (player.Experience < 0) player.Experience = 0;

        CheckLevelUp(player);
    }

    /// <summary>
    /// Checks whether the player has enough XP to level up (loops to handle multiple levels at once).
    /// Sends updated experience and, on level-up, refreshes the map player list.
    /// </summary>
    private static void CheckLevelUp(Player player)
    {
        byte numLevel = 0;

        while (player.Experience >= player.ExpNeeded)
        {
            numLevel++;
            var expRest = player.Experience - player.ExpNeeded;

            player.Level++;
            player.Points += NumPoints;
            player.Experience = expRest;
        }

        PlayerSender.PlayerExperience(player);
        if (numLevel > 0) MapSender.MapPlayers(player);
    }

    /// <summary>
    /// Splits <paramref name="value"/> XP across the player's party using a level-difference
    /// weight so that large level gaps reduce the share a member receives. The remaining XP
    /// after distributing to members is awarded to <paramref name="player"/> directly.
    /// </summary>
    private static void PartySplitXp(Player player, int value)
    {
        var diff = new double[player.Party.Count];
        double diffSum = 0;

        // Compute a weight for each party member based on their level difference.
        for (byte i = 0; i < player.Party.Count; i++)
        {
            var difference = Math.Abs(player.Level - player.Party[i].Level);

            // Scaling constant: larger gaps reduce the weight exponentially.
            double k;
            if (difference < 3) k = 1.15;
            else if (difference < 6) k = 1.55;
            else if (difference < 10) k = 1.85;
            else k = 2.3;

            diff[i] = 1 / Math.Pow(k, Math.Min(15, difference));
            diffSum += diff[i];
        }

        // Distribute XP to party members; balance weights when their sum exceeds 100 %.
        var experienceSum = 0;
        for (byte i = 0; i < player.Party.Count; i++)
        {
            if (diffSum > 1) diff[i] *= 1 / diffSum;

            var givenExperience = (int)(value / 2 * diff[i]);
            experienceSum += givenExperience;

            GiveExperience(player.Party[i], givenExperience);
            PlayerSender.PlayerExperience(player.Party[i]);
        }

        // Award the remainder to the triggering player.
        player.Experience += value - experienceSum;
        CheckLevelUp(player);
        PlayerSender.PlayerExperience(player);
    }
}
