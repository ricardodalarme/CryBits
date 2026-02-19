using System;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using static CryBits.Utils;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that owns all player movement: direction changes, tile-by-tile
/// movement, and map-transition warps.
/// </summary>
internal static class MovementSystem
{
    /// <summary>
    /// Validates and applies a direction change for <paramref name="player"/>,
    /// broadcasting it to the map. No-ops while the player is loading a new map.
    /// </summary>
    public static void ChangeDirection(Player player, Direction direction)
    {
        if (direction < Direction.Up || direction > Direction.Right) return;
        if (player.GettingMap) return;

        player.Direction = direction;
        PlayerSender.PlayerDirection(player);
    }

    /// <summary>
    /// Attempts to move <paramref name="player"/> one tile in their current direction.
    /// Handles map link boundaries, tile blocking, warp tile attributes, and
    /// cancels any active trade or shop session before moving.
    /// </summary>
    public static void Move(Player player, byte movement)
    {
        byte nextX = player.X, nextY = player.Y;
        byte oldX = player.X, oldY = player.Y;
        var link = TempMap.List.Get(player.Map.Data.Link[(byte)player.Direction].GetId());
        var secondMovement = false;

        if (movement < 1 || movement > 2) return;
        if (player.GettingMap) return;

        TradeSystem.Leave(player);
        ShopSystem.Leave(player);

        NextTile(player.Direction, ref nextX, ref nextY);

        // Map link boundary
        if (CryBits.Entities.Map.Map.OutLimit(nextX, nextY))
        {
            if (link != null)
                switch (player.Direction)
                {
                    case Direction.Up:
                        Warp(player, link, oldX, CryBits.Entities.Map.Map.Height - 1);
                        return;
                    case Direction.Down:
                        Warp(player, link, oldX, 0);
                        return;
                    case Direction.Right:
                        Warp(player, link, 0, oldY);
                        return;
                    case Direction.Left:
                        Warp(player, link, CryBits.Entities.Map.Map.Width - 1, oldY);
                        return;
                }
            else
            {
                PlayerSender.PlayerPosition(player);
                return;
            }
        }
        else if (!player.Map.TileBlocked(oldX, oldY, player.Direction))
        {
            player.X = nextX;
            player.Y = nextY;
        }

        // Tile attributes
        var tile = player.Map.Data.Attribute[nextX, nextY];
        switch ((TileAttribute)tile.Type)
        {
            case TileAttribute.Warp:
                if (tile.Data4 > 0) player.Direction = (Direction)tile.Data4 - 1;
                Warp(player, TempMap.List.Get(new Guid(tile.Data1)), (byte)tile.Data2, (byte)tile.Data3);
                secondMovement = true;
                break;
        }

        if (!secondMovement && (oldX != player.X || oldY != player.Y))
            PlayerSender.PlayerMove(player, movement);
        else
            PlayerSender.PlayerPosition(player);
    }

    /// <summary>
    /// Teleports <paramref name="player"/> to the specified position on <paramref name="map"/>.
    /// Cancels any active trade or shop, clamps coordinates to map bounds, and sends a full
    /// map data refresh when the destination map differs from the current one or
    /// <paramref name="needUpdate"/> is true.
    /// </summary>
    public static void Warp(Player player, TempMap map, byte x, byte y, bool needUpdate = false)
    {
        var oldMap = player.Map;

        TradeSystem.Leave(player);
        ShopSystem.Leave(player);

        if (map == null) return;
        if (x >= CryBits.Entities.Map.Map.Width) x = CryBits.Entities.Map.Map.Width - 1;
        if (y >= CryBits.Entities.Map.Map.Height) y = CryBits.Entities.Map.Map.Height - 1;

        player.Map = map;
        player.X = x;
        player.Y = y;

        if (oldMap != map || needUpdate)
        {
            PlayerSender.PlayerLeaveMap(player, oldMap);
            player.GettingMap = true;
            MapSender.MapRevision(player, map.Data);
            MapSender.MapItems(player, map);
            NpcSender.MapNpcs(player, map);
        }
        else
            PlayerSender.PlayerPosition(player);
    }
}
