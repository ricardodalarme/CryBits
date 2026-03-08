using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Movement;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Utils;

namespace CryBits.Client.Worlds;

/// <summary>
/// Runtime state for the current map: the static data from the server plus the live NPC entity array.
/// </summary>
internal class ClientMap(Map data, World world)
{
    public readonly Map Data = data;
    public Entity[] Npcs = [];

    private bool HasNpc(byte x, byte y)
    {
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
