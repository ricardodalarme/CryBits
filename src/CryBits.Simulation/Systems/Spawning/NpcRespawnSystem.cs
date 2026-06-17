using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Spawners;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Spawning;

public sealed class NpcRespawnSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly List<Entry> _pendingRespawns = [];
    private readonly record struct Entry(Guid MapId, byte NpcIndex, long RespawnTick);

    public void Execute(World world, Tick tick)
    {
        // Register pending respawns from NpcDiedEvent
        foreach (var ev in tick.Events.Events)
        {
            if (ev is not NpcDiedEvent died) continue;

            var npcData = catalog.Npcs.Get(died.NpcDefId);
            if (npcData == null) continue;

            _pendingRespawns.Add(new Entry(
                died.MapId, died.NpcIndex,
                tick.TickNumber + npcData.SpawnTime * TicksPerSecond));
        }

        // Process pending respawns
        for (var i = _pendingRespawns.Count - 1; i >= 0; i--)
        {
            var entry = _pendingRespawns[i];
            if (tick.TickNumber < entry.RespawnTick) continue;

            var map = world.Maps.Get(entry.MapId);
            if (map == null || !map.HasPlayers(world.Entities))
            {
                _pendingRespawns.RemoveAt(i);
                continue;
            }

            NpcSpawner.Spawn(world, catalog, entry.MapId, entry.NpcIndex);
            _pendingRespawns.RemoveAt(i);
        }
    }
}
