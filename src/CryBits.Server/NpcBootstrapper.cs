using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System;

namespace CryBits.Server;

internal static class NpcBootstrapper
{
    public static void Spawn(World world, EntityId npcId)
    {
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;
        var npcSpawn = map.Data.Npc[npcState.Index];

        if (npcSpawn.Spawn)
        {
            SpawnAt(world, npcId, npcSpawn.X, npcSpawn.Y);
            return;
        }

        for (byte i = 0; i < 50; i++)
        {
            var x = (byte)Random.Shared.Next(0, Map.Width - 1);
            var y = (byte)Random.Shared.Next(0, Map.Height - 1);

            if (npcSpawn.Zone > 0 &&
                map.Data.Attribute[x, y].Zone != npcSpawn.Zone)
                continue;

            if (!map.Data.TileBlocked(x, y))
            {
                SpawnAt(world, npcId, x, y);
                return;
            }
        }

        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (!map.Data.TileBlocked(x, y))
                {
                    if (npcSpawn.Zone > 0 &&
                        map.Data.Attribute[x, y].Zone != npcSpawn.Zone)
                        continue;

                    SpawnAt(world, npcId, x, y);
                    return;
                }
    }

    private static void SpawnAt(World world, EntityId npcId, byte x, byte y, Direction direction = 0)
    {
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var vitals = e.Get<Vitals>()!;
        var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);

        npcState.Alive = true;
        pos.X = x;
        pos.Y = y;
        pos.Direction = direction;
        vitals.Hp = npcData.Vital[(byte)Vital.Hp];
        vitals.Mp = npcData.Vital[(byte)Vital.Mp];

        world.Dirty.Mark<NpcState>(npcId);
        world.Dirty.Mark<Position>(npcId);
        world.Dirty.Mark<Vitals>(npcId);
    }
}
