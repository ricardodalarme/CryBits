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
        var e = world.Entities.Get(entityId);
        if (e == null) return;

        if (direction is < Direction.Up or > Direction.Right) return;
        if (e.Has<MapLoadingTag>()) return;

        world.Update<Position>(entityId, pos => pos with { Direction = direction });
    }

    private void Move(World world, Tick tick, EntityId entityId, CommonMovement movement)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var pos = e.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;

        byte oldX = pos.X, oldY = pos.Y;
        var (nextX, nextY) = pos.Direction.NextTile(pos.X, pos.Y);
        var link = world.Maps.Get(map.Data.LinkIds[(byte)pos.Direction]);

        if (movement is < CommonMovement.Walking or > CommonMovement.Moving) return;
        if (e.Has<MapLoadingTag>()) return;

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
                return;
        }
        else if (Map.OutLimit(nextX, nextY))
            return;
        else if (!map.TileBlocked(oldX, oldY, pos.Direction, world.Entities))
        {
            world.Update<Position>(entityId, p => p with { X = nextX, Y = nextY, Direction = p.Direction });

            if (e.Has<PlayerTag>())
                tick.Events.Emit(new PlayerStartedMovingEvent(tick.TickNumber, entityId));
        }

        if (e.Has<PlayerTag>())
        {
            var tile = map.Data.Attribute[nextX, nextY];
            if ((TileAttribute)tile.Type == TileAttribute.Warp)
            {
                var warpDir = tile.Data4 > 0 ? (Direction)tile.Data4 - 1 : pos.Direction;
                world.Update<Position>(entityId, p => p with { Direction = warpDir, X = p.X, Y = p.Y });
                Warp(world, tick, entityId, new Guid(tile.Data1), (byte)tile.Data2, (byte)tile.Data3);
            }
        }
    }

    private void Warp(World world, Tick tick, EntityId entityId, Guid mapId, byte x, byte y)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var pos = e.Get<Position>()!;

        var oldMapId = pos.MapId;

        if (!world.Maps.TryGetValue(mapId, out var map)) return;
        if (x >= Map.Width) x = Map.Width - 1;
        if (y >= Map.Height) y = Map.Height - 1;

        world.Update<Position>(entityId, p => p with { MapId = map.Id, X = x, Y = y });

        var needsMapData = oldMapId != map.Id;
        if (needsMapData)
            world.Set(entityId, new MapLoadingTag());

        if (e.Has<PlayerTag>())
            tick.Events.Emit(new PlayerWarpedEvent(tick.TickNumber, entityId, oldMapId, map.Id, needsMapData));
    }
}
