using CryBits.Server.Simulation.Core;
using CryBits.Server.World;
using System;

namespace CryBits.Server.Systems;

/// <summary>
/// Tick-driven system that handles NPC respawning.
/// Iterates all maps and spawns dead NPCs whose spawn timer has elapsed.
/// </summary>
internal sealed class SpawnSystem : ISimulationSystem
{
    public static SpawnSystem Instance { get; } = new();

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers()) continue;

            foreach (var npc in map.Npc)
            {
                if (npc.Alive) continue;
                if (Environment.TickCount64 > npc.SpawnTimer + npc.Data.SpawnTime * 1000)
                    NpcBrainSystem.Instance.Spawn(npc);
            }
        }
    }
}
