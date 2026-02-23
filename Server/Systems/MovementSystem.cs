using System;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using static CryBits.Utils;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that owns all player movement: direction changes, tile-by-tile
/// movement, and map-transition warps.
/// </summary>
internal static class MovementSystem
{
    /// <summary>Validates and applies a direction change for <paramref name="player"/>.</summary>
    public static void ChangeDirection(Player player, Direction direction)
    {
        if (direction < Direction.Up || direction > Direction.Right) return;
        if (player.Has<LoadingMapTag>()) return;

        player.Get<DirectionComponent>().Value = direction;
        PlayerSender.PlayerDirection(player);
    }

    /// <summary>
    /// Attempts to move <paramref name="player"/> one tile in their current direction.
    /// Handles map link boundaries, tile blocking, and warp tile attributes.
    /// </summary>
    public static void Move(Player player, byte movement)
    {
        var world = ServerContext.Instance.World;
        var pos   = world.Get<PositionComponent>(player.EntityId);
        var dir   = world.Get<DirectionComponent>(player.EntityId);

        if (player.Has<LoadingMapTag>()) return;
        if (movement < 1 || movement > 2) return;

        var map    = player.MapInstance;
        var link   = GameWorld.Current.Maps.Get(map.Data.Link[(byte)dir.Value].GetId());
        var oldX   = pos.X;
        var oldY   = pos.Y;
        byte nextX = pos.X, nextY = pos.Y;
        var secondMovement = false;

        TradeSystem.Leave(player);
        ShopSystem.Leave(player);

        NextTile(dir.Value, ref nextX, ref nextY);

        // Map link boundary
        if (CryBits.Entities.Map.Map.OutLimit(nextX, nextY))
        {
            if (link != null)
                switch (dir.Value)
                {
                    case Direction.Up:    Warp(player, link, oldX, CryBits.Entities.Map.Map.Height - 1); return;
                    case Direction.Down:  Warp(player, link, oldX, 0);                                   return;
                    case Direction.Right: Warp(player, link, 0, oldY);                                   return;
                    case Direction.Left:  Warp(player, link, CryBits.Entities.Map.Map.Width - 1, oldY);  return;
                }
            else
            {
                PlayerSender.PlayerPosition(player);
                return;
            }
        }
        else if (!map.TileBlocked(oldX, oldY, dir.Value))
        {
            pos.X = nextX;
            pos.Y = nextY;
        }

        // Tile attributes
        var tile = map.Data.Attribute[nextX, nextY];
        switch ((TileAttribute)tile.Type)
        {
            case TileAttribute.Warp:
                if (tile.Data4 > 0) dir.Value = (Direction)(tile.Data4 - 1);
                Warp(player, GameWorld.Current.Maps.Get(new Guid(tile.Data1)),
                    (byte)tile.Data2, (byte)tile.Data3);
                secondMovement = true;
                break;
        }

        if (!secondMovement && (oldX != pos.X || oldY != pos.Y))
            PlayerSender.PlayerMove(player, movement);
        else
            PlayerSender.PlayerPosition(player);
    }

    /// <summary>
    /// Teleports <paramref name="player"/> to the specified position on <paramref name="mapInstance"/>.
    /// </summary>
    public static void Warp(Player player, MapInstance? mapInstance, byte x, byte y, bool needUpdate = false)
    {
        if (mapInstance == null) return;

        var world   = ServerContext.Instance.World;
        var pos     = world.Get<PositionComponent>(player.EntityId);
        var oldMapId = pos.MapId;

        TradeSystem.Leave(player);
        ShopSystem.Leave(player);

        if (x >= CryBits.Entities.Map.Map.Width)  x = (byte)(CryBits.Entities.Map.Map.Width  - 1);
        if (y >= CryBits.Entities.Map.Map.Height) y = (byte)(CryBits.Entities.Map.Map.Height - 1);

        pos.MapId = mapInstance.Data.Id;
        pos.X = x;
        pos.Y = y;

        if (oldMapId != mapInstance.Data.Id || needUpdate)
        {
            var oldMap = GameWorld.Current.Maps.Get(oldMapId);
            if (oldMap != null)
                PlayerSender.PlayerLeaveMap(player, oldMap);

            world.Add(player.EntityId, new LoadingMapTag());
            MapSender.MapRevision(player, mapInstance.Data);
            MapSender.MapItems(player, mapInstance);
            NpcSender.MapNpcs(player, mapInstance);
        }
        else
            PlayerSender.PlayerPosition(player);
    }
}
