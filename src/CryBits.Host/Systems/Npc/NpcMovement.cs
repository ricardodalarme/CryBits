using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System;

namespace CryBits.Host.Systems.Npc;

internal static class NpcMovement
{
    internal static void TickMovement(World world, EntityId npcId, Tick tick)
    {
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var vitals = e.Get<Vitals>()!;
        var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);
        var map = world.Maps.Get(pos.MapId)!;

        var canMove = new bool[(byte)Direction.Count];
        var moved = false;
        var move = false;
        byte targetX = 0, targetY = 0;

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value);
            if (targetE != null)
            {
                var targetPos = targetE.Get<Position>()!;
                targetX = targetPos.X;
                targetY = targetPos.Y;
                move = true;
            }
        }

        if (!npcState.TargetId.HasValue && map.Data.Npc[npcState.Index].Zone > 0 &&
            map.Data.Attribute[pos.X, pos.Y].Zone != map.Data.Npc[npcState.Index].Zone)
        {
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    if (map.Data.Attribute[x, y].Zone == map.Data.Npc[npcState.Index].Zone &&
                        !map.Data.TileBlocked(x, y))
                    {
                        targetX = x;
                        targetY = y;
                        move = true;
                        break;
                    }
        }

        if (move)
        {
            if (vitals.Hp > npcData.Vital[(byte)Vital.Hp] * (npcData.FleeHealth / 100.0))
            {
                canMove[(byte)Direction.Up] = pos.Y > targetY;
                canMove[(byte)Direction.Down] = pos.Y < targetY;
                canMove[(byte)Direction.Left] = pos.X > targetX;
                canMove[(byte)Direction.Right] = pos.X < targetX;
            }
            else
            {
                canMove[(byte)Direction.Up] = pos.Y < targetY;
                canMove[(byte)Direction.Down] = pos.Y > targetY;
                canMove[(byte)Direction.Left] = pos.X < targetX;
                canMove[(byte)Direction.Right] = pos.X > targetX;
            }

            if (Random.Shared.Next(0, 2) == 0)
            {
                for (byte d = 0; d < (byte)Direction.Count; d++)
                    if (!moved && canMove[d] && Move(world, npcId, (Direction)d))
                        moved = true;
            }
            else
            {
                for (short d = (byte)Direction.Count - 1; d >= 0; d--)
                    if (!moved && canMove[d] && Move(world, npcId, (Direction)d))
                        moved = true;
            }
        }

        if (npcData.Behaviour == (byte)Behaviour.Friendly || !npcState.TargetId.HasValue)
            if (Random.Shared.Next(0, 3) == 0 && !moved)
            {
                if (npcData.Movement == MovementStyle.MoveRandomly)
                    Move(world, npcId, (Direction)Random.Shared.Next(0, 4), 1, true);
                else if (npcData.Movement == MovementStyle.TurnRandomly)
                {
                    pos.Direction = (Direction)Random.Shared.Next(0, 4);
                    world.Dirty.Mark<Position>(npcId);
                }
            }
    }

    private static bool Move(World world, EntityId npcId, Direction direction, byte movement = 1, bool checkZone = false)
    {
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;

        byte nextX = pos.X, nextY = pos.Y;

        pos.Direction = direction;
        direction.NextTile(ref nextX, ref nextY);

        if (Map.OutLimit(nextX, nextY)) return false;
        if (map.TileBlocked(pos.X, pos.Y, direction, world.Entities)) return false;
        if (checkZone && map.Data.Attribute[nextX, nextY].Zone != map.Data.Npc[npcState.Index].Zone) return false;

        pos.X = nextX;
        pos.Y = nextY;
        world.Dirty.Mark<Position>(npcId);
        return true;
    }
}
