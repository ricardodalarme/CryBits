using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Server.Systems.Npc;
using CryBits.Server.World;
using CryBits.Simulation.Core;
using System;

namespace CryBits.Server.Systems.Spawning;

internal sealed class SpawnSystem : ISimulationSystem
{
    public static SpawnSystem Instance { get; } = new();

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers(world.Entities)) continue;

            foreach (var npcId in map.NpcIds)
            {
                var e = world.Entities.Get(npcId);
                if (e == null) continue;
                var npcState = e.Get<NpcState>();
                if (npcState == null || npcState.Alive) continue;
                var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);
                if (Environment.TickCount64 > npcState.SpawnTimer + npcData.SpawnTime * 1000)
                    NpcBrainSystem.Instance.Spawn(npcId);
            }
        }
    }
}
