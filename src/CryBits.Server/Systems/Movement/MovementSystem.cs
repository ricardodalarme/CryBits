using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using System;
using CryBits.Simulation.Core;
namespace CryBits.Server.Systems.Movement;

internal sealed class MovementSystem(
    PlayerSender playerSender,
    NpcSender npcSender,
    MapSender mapSender) : ISimulationSystem
{
    public static MovementSystem Instance { get; } = new(
        PlayerSender.Instance,
        NpcSender.Instance,
        MapSender.Instance);

    public void ChangeDirection(Player player, Direction direction)
    {
        if (direction is < Direction.Up or > Direction.Right) return;
        if (player.GettingMap) return;

        player.Direction = direction;
        playerSender.PlayerDirection(player);
    }

    public void Move(Player player, byte movement)
    {
        byte nextX = player.X, nextY = player.Y;
        byte oldX = player.X, oldY = player.Y;
        var link = GameWorld.Current.Maps.Get(player.MapInstance.Data.LinkIds[(byte)player.Direction]);
        var secondMovement = false;

        if (movement is < 1 or > 2) return;
        if (player.GettingMap) return;

        GameWorld.Current.CurrentTick?.Events.Emit(new PlayerStartedMovingEvent { PlayerId = player.Id });

        player.Direction.NextTile(ref nextX, ref nextY);

        if (Map.OutLimit(nextX, nextY))
        {
            if (link != null)
                switch (player.Direction)
                {
                    case Direction.Up:
                        Warp(player, link, oldX, Map.Height - 1);
                        return;
                    case Direction.Down:
                        Warp(player, link, oldX, 0);
                        return;
                    case Direction.Right:
                        Warp(player, link, 0, oldY);
                        return;
                    case Direction.Left:
                        Warp(player, link, Map.Width - 1, oldY);
                        return;
                }
            else
            {
                playerSender.PlayerPosition(player);
                return;
            }
        }
        else if (!player.MapInstance.TileBlocked(oldX, oldY, player.Direction))
        {
            player.X = nextX;
            player.Y = nextY;
        }

        var tile = player.MapInstance.Data.Attribute[nextX, nextY];
        switch ((TileAttribute)tile.Type)
        {
            case TileAttribute.Warp:
                if (tile.Data4 > 0) player.Direction = (Direction)tile.Data4 - 1;
                Warp(player, GameWorld.Current.Maps.Get(new Guid(tile.Data1)), (byte)tile.Data2, (byte)tile.Data3);
                secondMovement = true;
                break;
        }

        if (!secondMovement && (oldX != player.X || oldY != player.Y))
            playerSender.PlayerMove(player, movement);
        else
            playerSender.PlayerPosition(player);
    }

    public void Warp(Player player, MapInstance mapInstance, byte x, byte y, bool needUpdate = false)
    {
        var oldMap = player.MapInstance;

        if (mapInstance == null) return;
        if (x >= Map.Width) x = Map.Width - 1;
        if (y >= Map.Height) y = Map.Height - 1;

        player.MapInstance = mapInstance;
        player.X = x;
        player.Y = y;

        GameWorld.Current.CurrentTick?.Events.Emit(new PlayerWarpedEvent { PlayerId = player.Id, OldMapId = oldMap.Id, NewMapId = mapInstance.Id });

        if (oldMap != mapInstance || needUpdate)
        {
            playerSender.PlayerLeaveMap(player, oldMap.Id);
            player.GettingMap = true;
            mapSender.MapRevision(player, mapInstance.Data);
            mapSender.MapItems(player, mapInstance);
            npcSender.MapNpcs(player, mapInstance);
        }
        else
            playerSender.PlayerPosition(player);
    }

    public void Execute(GameWorld world, Tick tick) { }
}
