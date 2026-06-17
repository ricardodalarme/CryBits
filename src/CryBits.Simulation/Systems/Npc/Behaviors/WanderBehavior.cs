using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class WanderBehavior : INpcBehavior
{
    public Intent? GetNextAction(World world, EntityState entity, Definitions.Npcs.Npc npcData, Tick tick)
    {
        if (Random.Shared.Next(0, 3) != 0) return null;

        var npcState = entity.Get<NpcState>()!;
        var pos = entity.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;
        var npcSpawn = map.Data.Npc[npcState.Index];

        byte targetX = 0, targetY = 0;
        var move = false;

        if (npcSpawn.Zone > 0 && map.Data.Attribute[pos.X, pos.Y].Zone != npcSpawn.Zone)
        {
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    if (map.Data.Attribute[x, y].Zone == npcSpawn.Zone &&
                        !map.Data.TileBlocked(x, y))
                    {
                        targetX = x;
                        targetY = y;
                        move = true;
                        break;
                    }
        }

        if (!move)
        {
            var dir = (Direction)Random.Shared.Next(0, 4);
            return new MoveIntent(entity.Id, dir, CryBits.Definitions.Common.Movement.Walking);
        }

        Direction chosen;
        if (pos.X > targetX)
            chosen = Direction.Left;
        else if (pos.X < targetX)
            chosen = Direction.Right;
        else if (pos.Y > targetY)
            chosen = Direction.Up;
        else
            chosen = Direction.Down;

        return new MoveIntent(entity.Id, chosen, CryBits.Definitions.Common.Movement.Walking);
    }
}
