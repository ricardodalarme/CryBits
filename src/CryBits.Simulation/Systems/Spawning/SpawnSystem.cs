using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Spawning;

public sealed class SpawnSystem(DefinitionCatalog catalog) : ISimulationSystem
{


    public void Execute(World world, Tick tick)
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
                var npcData = catalog.Npcs.Get(npcState.NpcDefId);
                if (tick.TickNumber > npcState.SpawnTimer + npcData.SpawnTime * TicksPerSecond)
                    tick.Events.Emit(new NpcRespawnEvent { NpcInstanceId = npcId.Value });
            }
        }
    }
}
