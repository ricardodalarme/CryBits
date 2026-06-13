using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System;
using System.Linq;

namespace CryBits.Simulation.Systems.Movement;

public sealed class MovementSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is MoveIntent move)
            {
                ChangeDirection(world, move.SourceEntityId, move.Direction);
                Move(world, move.SourceEntityId, move.Movement);
            }
        }

        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is PlayerRespawnEvent respawn)
                Warp(world, new EntityId(respawn.PlayerId), respawn.MapId, respawn.X, respawn.Y, true);
        }
    }

    private void ChangeDirection(World world, EntityId entityId, Direction direction)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var combat = e.Get<CombatState>()!;

        if (direction is < Direction.Up or > Direction.Right) return;
        if (combat.GettingMap) return;

        pos.Direction = direction;
        world.Dirty.Mark<Position>(entityId);
    }

    private void Move(World world, EntityId entityId, byte movement)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var combat = e.Get<CombatState>()!;
        var map = world.Maps.Get(pos.MapId)!;

        byte nextX = pos.X, nextY = pos.Y;
        byte oldX = pos.X, oldY = pos.Y;
        var link = world.Maps.Get(map.Data.LinkIds[(byte)pos.Direction]);

        if (movement is < 1 or > 2) return;
        if (combat.GettingMap) return;

        world.CurrentTick?.Events.Emit(new PlayerStartedMovingEvent { PlayerId = entityId.Value });

        pos.Direction.NextTile(ref nextX, ref nextY);

        if (Map.OutLimit(nextX, nextY))
        {
            if (link != null)
                switch (pos.Direction)
                {
                    case Direction.Up:
                        Warp(world, entityId, link.Id, oldX, Map.Height - 1);
                        return;
                    case Direction.Down:
                        Warp(world, entityId, link.Id, oldX, 0);
                        return;
                    case Direction.Right:
                        Warp(world, entityId, link.Id, 0, oldY);
                        return;
                    case Direction.Left:
                        Warp(world, entityId, link.Id, Map.Width - 1, oldY);
                        return;
                }
            else
            {
                world.Dirty.Mark<Position>(entityId);
                return;
            }
        }
        else if (!map.TileBlocked(oldX, oldY, pos.Direction, world.Entities))
        {
            pos.X = nextX;
            pos.Y = nextY;
        }

        var tile = map.Data.Attribute[nextX, nextY];
        if ((TileAttribute)tile.Type == TileAttribute.Warp)
        {
            if (tile.Data4 > 0) pos.Direction = (Direction)tile.Data4 - 1;
            Warp(world, entityId, new Guid(tile.Data1), (byte)tile.Data2, (byte)tile.Data3);
        }
        else if (oldX != pos.X || oldY != pos.Y)
            world.Dirty.Mark<Position>(entityId);
    }

    private void Warp(World world, EntityId entityId, Guid mapId, byte x, byte y, bool needUpdate = false)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var combat = e.Get<CombatState>()!;

        var oldMapId = pos.MapId;

        if (!world.Maps.TryGetValue(mapId, out var map)) return;
        if (x >= Map.Width) x = Map.Width - 1;
        if (y >= Map.Height) y = Map.Height - 1;

        pos.MapId = map.Id;
        pos.X = x;
        pos.Y = y;

        var needsMapData = needUpdate || oldMapId != map.Id;
        if (needsMapData)
            combat.GettingMap = true;

        world.CurrentTick?.Events.Emit(new PlayerWarpedEvent
        {
            PlayerId = entityId.Value,
            OldMapId = oldMapId,
            NewMapId = map.Id,
            NeedsMapData = needsMapData
        });

        world.Dirty.Mark<Position>(entityId);
    }
}
