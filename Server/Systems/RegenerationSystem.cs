using System;
using CryBits.Enums;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Formulas;
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
    /// <summary>Regenerates vitals for a player entity.</summary>
    public static void Tick(Player player)
    {
        if (Environment.TickCount64 <= Loop.TimerRegeneration + 5000) return;

        var vitals = player.Get<VitalsComponent>();

        for (byte v = 0; v < (byte)Vital.Count; v++)
        {
            if (vitals.Values[v] >= player.MaxVital(v)) continue;

            vitals.Values[v] += player.Regeneration(v);
            if (vitals.Values[v] > player.MaxVital(v)) vitals.Values[v] = player.MaxVital(v);

            PlayerSender.PlayerVitals(player);
        }
    }

    /// <summary>Regenerates vitals for an NPC entity. Only runs when the NPC is alive.</summary>
    public static void Tick(int npcEntityId)
    {
        var world    = ServerContext.Instance.World;
        var npcState = world.Get<NpcStateComponent>(npcEntityId);

        if (!npcState.Alive) return;
        if (Environment.TickCount64 <= Loop.TimerRegeneration + 5000) return;

        var npcData = world.Get<NpcDataComponent>(npcEntityId);
        var vitals  = world.Get<VitalsComponent>(npcEntityId);

        for (byte v = 0; v < (byte)Vital.Count; v++)
        {
            if (vitals.Values[v] >= npcData.Data.Vital[v]) continue;

            vitals.Values[v] += VitalFormulas.NpcRegeneration(
                (Vital)v,
                npcData.Data.Vital[v],
                npcData.Data.Attribute[(byte)Attribute.Vitality],
                npcData.Data.Attribute[(byte)Attribute.Intelligence]);

            if (vitals.Values[v] > npcData.Data.Vital[v])
                vitals.Values[v] = npcData.Data.Vital[v];

            NpcSender.MapNpcVitals(npcEntityId);
        }
    }
}
