using CryBits.Definitions.Catalog;
using CryBits.Simulation.Core;
using CryBits.Simulation.Spawners;

namespace CryBits.Host.Core;

internal sealed class WorldInitializer(WorldHost host, DefinitionCatalog catalog)
{
    public void Initialize()
    {
        host.Maps.Clear();
        foreach (var mapDef in catalog.Maps.Values)
        {
            var mapState = new MapState(mapDef.Id, mapDef);
            mapState.SpawnItems(host.Entities);
            host.Maps.Add(mapDef.Id, mapState);
            for (byte i = 0; i < mapDef.Npc.Count; i++)
                NpcSpawner.Spawn(host.Simulation, catalog, mapState.Id, i);
        }
    }
}
