using System;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Logic;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

/// <summary>
/// Tick-driven system that regenerates player and NPC vitals.
/// Called every 500ms from the main loop; regeneration itself fires every 5 seconds
/// controlled by <see cref="Loop.TimerRegeneration"/>.
/// </summary>
internal static class RegenerationSystem
{
    /// <summary>
    /// Regenerates vitals for a player. Sends updated vitals to the map when any vital changes.
    /// </summary>
    public static void Tick(Player player)
    {
        if (Environment.TickCount64 <= Loop.TimerRegeneration + 5000) return;

        for (byte v = 0; v < (byte)Vital.Count; v++)
        {
            if (player.Vital[v] >= player.MaxVital(v)) continue;

            player.Vital[v] += player.Regeneration(v);
            if (player.Vital[v] > player.MaxVital(v)) player.Vital[v] = player.MaxVital(v);

            PlayerSender.PlayerVitals(player);
        }
    }

    /// <summary>
    /// Regenerates vitals for an NPC. Sends updated vitals to the map when any vital changes.
    /// Only runs when the NPC is alive.
    /// </summary>
    public static void Tick(NpcInstance npcInstance)
    {
        if (!npcInstance.Alive) return;
        if (Environment.TickCount64 <= Loop.TimerRegeneration + 5000) return;

        for (byte v = 0; v < (byte)Vital.Count; v++)
        {
            if (npcInstance.Vital[v] >= npcInstance.Data.Vital[v]) continue;

            npcInstance.Vital[v] += npcInstance.Regeneration(v);
            if (npcInstance.Vital[v] > npcInstance.Data.Vital[v]) npcInstance.Vital[v] = npcInstance.Data.Vital[v];

            NpcSender.MapNpcVitals(npcInstance);
        }
    }
}
