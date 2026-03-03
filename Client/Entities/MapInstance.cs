using System;
using System.Collections.Generic;
using Arch.Core;
using CryBits.Client.Components.Movement;
using CryBits.Client.Worlds;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Utils;

namespace CryBits.Client.Entities;

internal class MapInstance
{
    // Map collection
    public static Dictionary<Guid, MapInstance> List;

    // Map data
    public readonly Map Data;
    public Entity[] Npcs;

    public MapInstance(Map data)
    {
        Data = data;
    }

    private bool HasNpc(byte x, byte y)
    {
        var world = GameContext.Instance.World;
        for (byte i = 0; i < Npcs.Length; i++)
            if (Npcs[i] != Entity.Null)
            {
                ref var m = ref world.Get<MovementComponent>(Npcs[i]);
                if (m.TileX == x && m.TileY == y) return true;
            }

        return false;
    }

    private bool HasPlayer(short x, short y)
    {
        var world = GameContext.Instance.World;
        for (byte i = 0; i < Player.List.Count; i++)
        {
            var p = Player.List[i];
            if (p.MapInstance != this) continue;
            ref var m = ref world.Get<MovementComponent>(p.Entity);
            if (m.TileX == x && m.TileY == y) return true;
        }

        return false;
    }

    public bool TileBlocked(byte x, byte y, Direction direction)
    {
        byte nextX = x, nextY = y;

        // calculate the next tile in the given direction
        NextTile(direction, ref nextX, ref nextY);

        // if leaving map, check for a link
        if (Map.OutLimit(nextX, nextY)) return Data.Link[(byte)direction] == null;

        // check blocking attributes and occupants
        if (Data.Attribute[nextX, nextY].Type == (byte)TileAttribute.Block) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (HasPlayer(nextX, nextY) || HasNpc(nextX, nextY)) return true;
        return false;
    }
}
