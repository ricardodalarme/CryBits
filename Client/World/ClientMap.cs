using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Movement;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Utils;

namespace CryBits.Client.Worlds;

/// <summary>
/// Runtime state for the current map and its static data.
/// </summary>
internal class ClientMap(Map data)
{
    private static readonly QueryDescription _npcQuery =
        new QueryDescription().WithAll<MovementComponent, MapIdComponent, NpcTagComponent>();

    public readonly Map Data = data;

    private bool HasNpc(byte x, byte y)
    {
        var world = GameContext.Instance.World;
        var found = false;
        world.Query(in _npcQuery, (ref MovementComponent m, ref MapIdComponent mapId) =>
        {
            if (mapId.Value == Data.Id && m.TileX == x && m.TileY == y)
                found = true;
        });
        return found;
    }

    private bool HasPlayer(short x, short y)
    {
        var world = GameContext.Instance.World;
        var found = false;
        var query = new QueryDescription().WithAll<MovementComponent, MapIdComponent, PlayerTagComponent>();
        world.Query(in query, (ref MovementComponent m, ref MapIdComponent mapId) =>
        {
            if (mapId.Value == Data.Id && m.TileX == x && m.TileY == y)
                found = true;
        });
        return found;
    }

    public bool TileBlocked(byte x, byte y, Direction direction)
    {
        byte nextX = x, nextY = y;
        NextTile(direction, ref nextX, ref nextY);

        if (Map.OutLimit(nextX, nextY)) return Data.Link[(byte)direction] == null;

        if (Data.Attribute[nextX, nextY].Type == (byte)TileAttribute.Block) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (HasPlayer(nextX, nextY) || HasNpc(nextX, nextY)) return true;
        return false;
    }
}
