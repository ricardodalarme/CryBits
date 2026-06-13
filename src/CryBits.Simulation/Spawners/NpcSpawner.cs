using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System;

namespace CryBits.Simulation.Spawners;

public static class NpcSpawner
{
    public static EntityId Spawn(World world, DefinitionCatalog catalog, Guid mapId, byte npcIndex)
    {
        var map = world.Maps.Get(mapId);
        if (map == null) return default;

        var npcSpawn = map.Data.Npc[npcIndex];
        var npcData = catalog.Npcs.Get(npcSpawn.NpcId);
        if (npcData == null) return default;

        var entityId = world.Entities.Create();
        var entity = world.Entities.Get(entityId)!;

        var (x, y) = FindSpawnPosition(map, map.Data, npcSpawn);

        entity.Set(new NpcState
        {
            Index = npcIndex,
            NpcDefId = npcSpawn.NpcId,
            TargetId = null,
            AttackTimer = 0
        });

        entity.Set(new Position
        {
            X = x,
            Y = y,
            Direction = Direction.Down,
            MapId = mapId
        });

        entity.Set(new Vitals
        {
            Hp = npcData.Vital[(byte)Vital.Hp],
            Mp = npcData.Vital[(byte)Vital.Mp],
            MaxHp = npcData.Vital[(byte)Vital.Hp],
            MaxMp = npcData.Vital[(byte)Vital.Mp]
        });

        entity.Set(new CombatState());
        entity.Set(new NpcTag());

        world.Dirty.Mark<NpcState>(entityId);
        world.Dirty.Mark<Position>(entityId);
        world.Dirty.Mark<Vitals>(entityId);

        return entityId;
    }

    private static (byte x, byte y) FindSpawnPosition(MapState map, Map mapData, MapNpc npcSpawn)
    {
        if (npcSpawn.Spawn)
            return (npcSpawn.X, npcSpawn.Y);

        for (byte i = 0; i < 50; i++)
        {
            var x = (byte)Random.Shared.Next(0, Map.Width - 1);
            var y = (byte)Random.Shared.Next(0, Map.Height - 1);

            if (npcSpawn.Zone > 0 && mapData.Attribute[x, y].Zone != npcSpawn.Zone)
                continue;

            if (!mapData.TileBlocked(x, y))
                return (x, y);
        }

        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (!mapData.TileBlocked(x, y))
                {
                    if (npcSpawn.Zone > 0 && mapData.Attribute[x, y].Zone != npcSpawn.Zone)
                        continue;
                    return (x, y);
                }

        return (0, 0);
    }
}
