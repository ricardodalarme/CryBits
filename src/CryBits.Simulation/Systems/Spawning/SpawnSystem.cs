using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Spawners;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Spawning;

public sealed class SpawnSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers(world.Entities)) continue;

            for (var i = 0; i < map.NpcIds.Count; i++)
            {
                var e = world.Entities.Get(map.NpcIds[i]);
                var npcState = e?.Get<NpcState>();
                if (npcState == null || npcState.Alive) continue;

                var npcData = catalog.Npcs.Get(npcState.NpcDefId);
                if (tick.TickNumber > npcState.SpawnTimer + npcData.SpawnTime * TicksPerSecond)
                {
                    world.Entities.Destroy(map.NpcIds[i]);
                    map.NpcIds[i] = NpcSpawner.Spawn(world, catalog, map.Id, npcState.Index);
                }
            }
        }
    }
}
