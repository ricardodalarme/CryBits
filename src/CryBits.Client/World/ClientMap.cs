using CryBits.Client.Components;
using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;

namespace CryBits.Client.Worlds;

internal class ClientMap(Map data, World world)
{
    public readonly Map Data = data;

    private bool HasCollidable(byte x, byte y)
    {
        foreach (var state in world.All)
        {
            if (!state.Has<CollidableTag>()) continue;
            var movement = state.Get<MovementComponent>();
            if (movement != null && movement.TileX == x && movement.TileY == y)
                return true;
        }
        return false;
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
