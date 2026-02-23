using System;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Formulas;
using CryBits.Server.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that manages player experience, leveling, and party XP distribution.
/// </summary>
internal static class LevelingSystem
{
    /// <summary>Spends one attribute point for <paramref name="player"/> on the given attribute.</summary>
    internal static void AddPoint(Player player, byte attributeNum)
    {
        var pd   = player.Get<PlayerDataComponent>();
        var attr = player.Get<AttributeComponent>();

        if (pd.Points <= 0) return;

        attr.Values[attributeNum]++;
        pd.Points--;
        PlayerSender.PlayerExperience(player);
        MapSender.MapPlayers(player);
    }

    /// <summary>
    /// Grants <paramref name="value"/> experience to <paramref name="player"/>.
    /// Splits XP across party members when the player is in a party.
    /// </summary>
    public static void GiveExperience(Player player, int value)
    {
        var party = player.Get<PartyComponent>();

        if (party.MemberEntityIds.Count > 0 && value > 0)
            PartySplitXp(player, value);
        else
            player.Get<PlayerDataComponent>().Experience += value;

        var pd = player.Get<PlayerDataComponent>();
        if (pd.Experience < 0) pd.Experience = 0;

        CheckLevelUp(player);
    }

    private static void CheckLevelUp(Player player)
    {
        var pd = player.Get<PlayerDataComponent>();
        byte numLevel = 0;

        while (pd.Experience >= player.ExpNeeded)
        {
            numLevel++;
            var expRest = pd.Experience - player.ExpNeeded;
            pd.Level++;
            pd.Points += Config.NumPoints;
            pd.Experience = expRest;
        }

        PlayerSender.PlayerExperience(player);
        if (numLevel > 0) MapSender.MapPlayers(player);
    }

    private static void PartySplitXp(Player player, int value)
    {
        var party = player.Get<PartyComponent>();
        var world = ECS.ServerContext.Instance.World;
        var diff  = new double[party.MemberEntityIds.Count];
        double diffSum = 0;

        var pd = player.Get<PlayerDataComponent>();

        for (byte i = 0; i < party.MemberEntityIds.Count; i++)
        {
            var memberPd = world.Get<PlayerDataComponent>(party.MemberEntityIds[i]);
            var difference = Math.Abs(pd.Level - memberPd.Level);
            diff[i] = LevelingFormulas.PartyXpWeight(difference);
            diffSum += diff[i];
        }

        var experienceSum = 0;
        for (byte i = 0; i < party.MemberEntityIds.Count; i++)
        {
            if (diffSum > 1) diff[i] *= 1 / diffSum;

            var givenExperience = (int)(value / 2 * diff[i]);
            experienceSum += givenExperience;

            var memberSession = world.Get<ECS.Components.SessionComponent>(party.MemberEntityIds[i]).Session;
            if (memberSession.IsPlaying)
            {
                GiveExperience(memberSession.Character!, givenExperience);
                PlayerSender.PlayerExperience(memberSession.Character!);
            }
        }

        pd.Experience += value - experienceSum;
        CheckLevelUp(player);
        PlayerSender.PlayerExperience(player);
    }
}
