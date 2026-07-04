using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Spawners;

public static class NpcSpawner
{
    public static EntityId Spawn(World world, Guid mapId, int npcIndex)
    {
        if (!world.MapDefs.TryGetValue(mapId, out var mapDef))
            return default;

        var npcSpawn = mapDef.Npc[npcIndex];
        var npcData = world.Catalog.Npcs.Get(npcSpawn.NpcId);
        if (npcData == null) return default;

        var entityId = world.Entities.Create();

        var (x, y) = FindSpawnPosition(mapDef, npcSpawn);

        world.Set(entityId, new NpcState(Index: npcIndex, NpcDefId: npcSpawn.NpcId));
        world.Set(entityId, new Position(MapId: mapId, X: x, Y: y, Direction: Direction.Down));
        world.Set(entityId, new Vitals(
            Hp: npcData.Vital[(byte)Vital.Hp],
            Mp: npcData.Vital[(byte)Vital.Mp],
            MaxHp: npcData.Vital[(byte)Vital.Hp],
            MaxMp: npcData.Vital[(byte)Vital.Mp]
        ));
        world.Set(entityId, new LevelComponent(Level: 1));
        world.Set(entityId, new AttributesComponent((short[])npcData.Attribute.Clone()));
        world.Set(entityId, new AttackCooldown());
        world.Set(entityId, new NpcTag());
        world.Set(entityId, new CollidableTag());

        return entityId;
    }

    private static (int x, int y) FindSpawnPosition(Map mapData, MapNpc npcSpawn)
    {
        if (npcSpawn.Spawn)
            return (npcSpawn.X, npcSpawn.Y);

        var allChunks = mapData.Chunks.Values.Where(c => c.Tiles != null).ToList();
        if (allChunks.Count == 0) return (0, 0);

        var chunkSize = ChunkGrid.ChunkSize;
        var candidates = new List<(int x, int y)>();

        foreach (var chunk in allChunks)
        {
            for (var tx = 0; tx < chunkSize; tx++)
                for (var ty = 0; ty < chunkSize; ty++)
                    if (!chunk.Tiles![tx, ty].IsBlocked)
                    {
                        var wx = chunk.X * chunkSize + tx;
                        var wy = chunk.Y * chunkSize + ty;
                        candidates.Add((wx, wy));
                    }
        }

        if (candidates.Count == 0) return (0, 0);
        return candidates[Random.Shared.Next(candidates.Count)];
    }
}
