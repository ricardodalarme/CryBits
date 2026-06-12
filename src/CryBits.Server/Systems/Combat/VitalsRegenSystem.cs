using CryBits.Definitions.Characters;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.World;
using System;
using System.Linq;

namespace CryBits.Server.Systems.Combat;

internal sealed class VitalsRegenSystem(PlayerSender playerSender, NpcSender npcSender) : ISimulationSystem
{
    public static VitalsRegenSystem Instance { get; } = new(PlayerSender.Instance, NpcSender.Instance);

    private long _lastRegenTick;

    public void Execute(GameWorld world, Tick tick)
    {
        if (Environment.TickCount64 <= _lastRegenTick + 5000) return;
        _lastRegenTick = Environment.TickCount64;

        foreach (var session in world.Sessions.Where(a => a.IsPlaying))
        {
            var player = session.Character!;
            for (byte v = 0; v < (byte)Vital.Count; v++)
            {
                if (player.Vital[v] >= player.MaxVital(v)) continue;

                player.Vital[v] += player.Regeneration(v);
                if (player.Vital[v] > player.MaxVital(v)) player.Vital[v] = player.MaxVital(v);

                playerSender.PlayerVitals(player);
            }
        }

        foreach (var map in world.Maps.Values)
        {
            foreach (var npc in map.Npc)
            {
                if (!npc.Alive) continue;

                for (byte v = 0; v < (byte)Vital.Count; v++)
                {
                    if (npc.Vital[v] >= npc.Data.Vital[v]) continue;

                    npc.Vital[v] += npc.Regeneration(v);
                    if (npc.Vital[v] > npc.Data.Vital[v]) npc.Vital[v] = npc.Data.Vital[v];

                    npcSender.MapNpcVitals(npc);
                }
            }
        }
    }
}
