using CryBits.Definitions.Maps;
using CryBits.Definitions.Common;
using CryBits.Definitions.Characters;
using CryBits.Server.Entities;
using CryBits.Server.Network;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.World;
using System;


namespace CryBits.Server.Systems.Npc;

internal sealed class NpcBrainSystem(
    NpcSender npcSender,
    ChatSender chatSender,
    NetworkServer networkServer) : ISimulationSystem
{
    public static NpcBrainSystem Instance { get; } = new(
        NpcSender.Instance,
        ChatSender.Instance,
        NetworkServer.Instance);

    private long _lastTick;

    public void Execute(GameWorld world, Tick tick)
    {
        if (Environment.TickCount64 <= _lastTick + 500) return;
        _lastTick = Environment.TickCount64;

        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers()) continue;

            foreach (var npc in map.Npc)
            {
                if (!npc.Alive) continue;
                TickAlive(npc);
            }
        }
    }

    private void TickAlive(NpcInstance npc)
    {
        NpcTargeting.UpdateTarget(npc, chatSender);
        NpcMovement.TickMovement(npc, npcSender, chatSender);
    }

    internal void Spawn(NpcInstance npcInstance)
    {
        if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Spawn)
        {
            SpawnAt(npcInstance, npcInstance.MapInstance.Data.Npc[npcInstance.Index].X, npcInstance.MapInstance.Data.Npc[npcInstance.Index].Y);
            return;
        }

        for (byte i = 0; i < 50; i++)
        {
            var x = (byte)Random.Shared.Next(0, Map.Width - 1);
            var y = (byte)Random.Shared.Next(0, Map.Height - 1);

            if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone > 0 &&
                npcInstance.MapInstance.Data.Attribute[x, y].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone)
                continue;

            if (!npcInstance.MapInstance.Data.TileBlocked(x, y))
            {
                SpawnAt(npcInstance, x, y);
                return;
            }
        }

        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (!npcInstance.MapInstance.Data.TileBlocked(x, y))
                {
                    if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone > 0 &&
                        npcInstance.MapInstance.Data.Attribute[x, y].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone)
                        continue;

                    SpawnAt(npcInstance, x, y);
                    return;
                }
    }

    private void SpawnAt(NpcInstance npcInstance, byte x, byte y, Direction direction = 0)
    {
        npcInstance.Alive = true;
        npcInstance.X = x;
        npcInstance.Y = y;
        npcInstance.Direction = direction;
        for (byte i = 0; i < (byte)Vital.Count; i++) npcInstance.Vital[i] = npcInstance.Data.Vital[i];
        if (networkServer.Device != null) npcSender.MapNpc(npcInstance.MapInstance.Npc[npcInstance.Index]);
    }
}
