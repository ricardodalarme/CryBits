using CryBits.Definitions.Catalog;
using CryBits.Simulation.Spawners;

namespace CryBits.Host.Core;

internal sealed class WorldInitializer(WorldHost host)
{
    public void Initialize()
    {
        var world = host.Simulation;
        world.MapDefs.Clear();
        foreach (var mapDef in world.Catalog.Maps.Values)
        {
            world.MapDefs[mapDef.Id] = mapDef;
            for (var i = 0; i < mapDef.Npc.Count; i++)
                NpcSpawner.Spawn(world, mapDef.Id, i);
        }
    }
}
