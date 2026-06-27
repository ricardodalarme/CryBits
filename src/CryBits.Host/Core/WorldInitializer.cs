using CryBits.Definitions.Catalog;
using CryBits.Simulation.Spawners;

namespace CryBits.Host.Core;

internal sealed class WorldInitializer(WorldHost host, DefinitionCatalog catalog)
{
    public void Initialize()
    {
        var world = host.Simulation;
        world.MapDefs.Clear();
        foreach (var mapDef in catalog.Maps.Values)
        {
            world.MapDefs[mapDef.Id] = mapDef;
            for (int i = 0; i < mapDef.Npc.Count; i++)
                NpcSpawner.Spawn(world, catalog, mapDef.Id, i);
        }
    }
}
