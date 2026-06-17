using Arch.Core;
using CryBits.Client.Components.Movement;
using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
namespace CryBits.Client.Worlds;

/// <summary>
/// Runtime state for the current map: the static data from the server plus the live NPC entity array.
/// </summary>
internal class ClientMap(Map data, World world)
{
    private readonly QueryDescription _collidableQuery = new QueryDescription().WithAll<CollidableComponent, MovementComponent>();

    public readonly Map Data = data;

    private bool HasCollidable(byte x, byte y)
    {
        var found = false;
        world.Query(in _collidableQuery, (ref MovementComponent m) =>
        {
            if (m.TileX == x && m.TileY == y)
                found = true;
        });
        return found;
    }

    public bool TileBlocked(byte x, byte y, Direction direction)
    {
        var (nextX, nextY) = direction.NextTile(x, y);

        if (Map.OutLimit(nextX, nextY)) return Data.LinkIds[(byte)direction] == Guid.Empty;

        if (Data.Attribute[nextX, nextY].Type == (byte)TileAttribute.Block) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)direction.Reverse()]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (HasCollidable(nextX, nextY)) return true;
        return false;
    }
}
