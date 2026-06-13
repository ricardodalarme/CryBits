using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using System;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Movement;

internal sealed class MovementSystem(
    MapSender mapSender,
    NpcSender npcSender) : ISimulationSystem
{
    public static MovementSystem Instance { get; } = new(
        MapSender.Instance,
        NpcSender.Instance);

    public void ChangeDirection(EntityId entityId, Direction direction)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var combat = e.Get<CombatState>()!;

        if (direction is < Direction.Up or > Direction.Right) return;
        if (combat.GettingMap) return;

        pos.Direction = direction;
        world.Dirty.Mark<Position>(entityId);
    }

    public void Move(EntityId entityId, byte movement)
    {
        var world = GameWorld.Current;
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
                        Warp(entityId, link, oldX, Map.Height - 1);
                        return;
                    case Direction.Down:
                        Warp(entityId, link, oldX, 0);
                        return;
                    case Direction.Right:
                        Warp(entityId, link, 0, oldY);
                        return;
                    case Direction.Left:
                        Warp(entityId, link, Map.Width - 1, oldY);
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
            Warp(entityId, world.Maps.Get(new Guid(tile.Data1)), (byte)tile.Data2, (byte)tile.Data3);
        }
        else if (oldX != pos.X || oldY != pos.Y)
            world.Dirty.Mark<Position>(entityId);
    }

    public void Warp(EntityId entityId, MapInstance mapInstance, byte x, byte y, bool needUpdate = false)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var combat = e.Get<CombatState>()!;

        var oldMapId = pos.MapId;

        if (mapInstance == null) return;
        if (x >= Map.Width) x = Map.Width - 1;
        if (y >= Map.Height) y = Map.Height - 1;

        pos.MapId = mapInstance.Id;
        pos.X = x;
        pos.Y = y;

        world.CurrentTick?.Events.Emit(new PlayerWarpedEvent { PlayerId = entityId.Value, OldMapId = oldMapId, NewMapId = mapInstance.Id });

        if (oldMapId != mapInstance.Id || needUpdate)
        {
            combat.GettingMap = true;
            mapSender.MapRevision(entityId, mapInstance.Data);
            mapSender.MapItems(entityId, mapInstance);
            npcSender.MapNpcs(entityId, mapInstance);
        }

        world.Dirty.Mark<Position>(entityId);
    }

    public void Execute(GameWorld world, Tick tick) { }
}
