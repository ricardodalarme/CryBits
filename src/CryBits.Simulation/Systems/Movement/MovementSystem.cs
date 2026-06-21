using CryBits.Definitions.Common;
using CommonMovement = CryBits.Definitions.Common.Movement;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

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
                Move(world, tick, move.SourceEntityId, move.Movement);
            }
        }
    }

    private void ChangeDirection(World world, EntityId entityId, Direction direction)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;

        if (direction is < Direction.Up or > Direction.Right) return;
        if (pos.LoadingMap) return;

        pos.Direction = direction;
        world.MarkDirty<Position>(entityId);
    }

    private void Move(World world, Tick tick, EntityId entityId, CommonMovement movement)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;

        byte oldX = pos.X, oldY = pos.Y;
        var (nextX, nextY) = pos.Direction.NextTile(pos.X, pos.Y);
        var link = world.Maps.Get(map.Data.LinkIds[(byte)pos.Direction]);

        if (movement is < CommonMovement.Walking or > CommonMovement.Moving) return;
        if (pos.LoadingMap) return;

        if (e.Has<PlayerTag>() && Map.OutLimit(nextX, nextY))
        {
            if (link != null)
                switch (pos.Direction)
                {
                    case Direction.Up:
                        Warp(world, tick, entityId, link.Id, oldX, Map.Height - 1);
                        return;
                    case Direction.Down:
                        Warp(world, tick, entityId, link.Id, oldX, 0);
                        return;
                    case Direction.Right:
                        Warp(world, tick, entityId, link.Id, 0, oldY);
                        return;
                    case Direction.Left:
                        Warp(world, tick, entityId, link.Id, Map.Width - 1, oldY);
                        return;
                }
            else
            {
                world.MarkDirty<Position>(entityId);
                return;
            }
        }
        else if (Map.OutLimit(nextX, nextY))
        {
            world.MarkDirty<Position>(entityId);
            return;
        }
        else if (!map.TileBlocked(oldX, oldY, pos.Direction, world.Entities))
        {
            pos.X = nextX;
            pos.Y = nextY;

            if (e.Has<PlayerTag>())
                tick.Events.Emit(new PlayerStartedMovingEvent { PlayerId = entityId });
        }

        if (e.Has<PlayerTag>())
        {
            var tile = map.Data.Attribute[nextX, nextY];
            if ((TileAttribute)tile.Type == TileAttribute.Warp)
            {
                if (tile.Data4 > 0) pos.Direction = (Direction)tile.Data4 - 1;
                Warp(world, tick, entityId, new Guid(tile.Data1), (byte)tile.Data2, (byte)tile.Data3);
            }
        }

        if (oldX != pos.X || oldY != pos.Y)
            world.MarkDirty<Position>(entityId);
    }

    private void Warp(World world, Tick tick, EntityId entityId, Guid mapId, byte x, byte y)
    {
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;

        var oldMapId = pos.MapId;

        if (!world.Maps.TryGetValue(mapId, out var map)) return;
        if (x >= Map.Width) x = Map.Width - 1;
        if (y >= Map.Height) y = Map.Height - 1;

        pos.MapId = map.Id;
        pos.X = x;
        pos.Y = y;

        var needsMapData = oldMapId != map.Id;
        if (needsMapData)
            pos.LoadingMap = true;

        if (e.Has<PlayerTag>())
            tick.Events.Emit(new PlayerWarpedEvent
            {
                PlayerId = entityId,
                OldMapId = oldMapId,
                NewMapId = map.Id,
                NeedsMapData = needsMapData
            });

        world.MarkDirty<Position>(entityId);
    }
}
