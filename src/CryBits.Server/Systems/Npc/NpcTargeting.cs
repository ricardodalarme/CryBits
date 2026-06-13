using CryBits.Definitions.Npcs;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using System;
using System.Drawing;

namespace CryBits.Server.Systems.Npc;

internal static class NpcTargeting
{
    internal static void UpdateTarget(NpcInstance npc, ChatSender chatSender)
    {
        if (npc.Data.Behaviour == Behaviour.AttackOnSight && npc.Target == null)
            ScanForTarget(npc, chatSender);

        if (npc.Target != null)
        {
            if (npc.Target is Player p && !p.Session.IsPlaying || npc.Target.MapInstance != npc.MapInstance)
                npc.Target = null;
            else if (npc.Target is NpcInstance { Alive: false })
                npc.Target = null;
        }

        if (npc.Target != null)
        {
            var distance = Math.Sqrt(Math.Pow(npc.X - npc.Target.X, 2) + Math.Pow(npc.Y - npc.Target.Y, 2));
            if (npc.Data.Sight < distance)
                npc.Target = null;
        }
    }

    private static void ScanForTarget(NpcInstance npc, ChatSender chatSender)
    {
        short distance;

        foreach (var session in GameWorld.Current.Sessions)
        {
            if (!session.IsPlaying) continue;
            if (session.Character!.MapInstance != npc.MapInstance) continue;

            distance = (short)Math.Sqrt(Math.Pow(npc.X - session.Character.X, 2) +
                                        Math.Pow(npc.Y - session.Character.Y, 2));
            if (distance <= npc.Data.Sight)
            {
                npc.Target = session.Character;
                if (!string.IsNullOrEmpty(npc.Data.SayMsg))
                    chatSender.Message(session.Character, npc.Data.Name + ": " + npc.Data.SayMsg, Color.White);
                return;
            }
        }

        if (!npc.Data.AttackNpc) return;

        for (byte i = 0; i < npc.MapInstance.Npc.Length; i++)
        {
            if (i == npc.Index) continue;
            if (!npc.MapInstance.Npc[i].Alive) continue;
            if (npc.Data.IsAllied(npc.MapInstance.Npc[i].Data.Id)) continue;

            distance = (short)Math.Sqrt(Math.Pow(npc.X - npc.MapInstance.Npc[i].X, 2) +
                                        Math.Pow(npc.Y - npc.MapInstance.Npc[i].Y, 2));
            if (distance <= npc.Data.Sight)
            {
                npc.Target = npc.MapInstance.Npc[i];
                return;
            }
        }
    }
}
