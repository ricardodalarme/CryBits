using Arch.Core;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Utils;

namespace CryBits.Client.Worlds;

/// <summary>
/// Runtime state for the current map: the static tile/link data received from
/// the server, an index-keyed NPC entity array (used by server-protocol packet
/// handlers to look up NPCs by slot), and an O(1) spatial collision grid
/// maintained each frame by <see cref="Systems.Map.MapCollisionSystem"/>.
/// </summary>
internal class ClientMap(Map data)
{
    public readonly Map Data = data;

    /// <summary>
    /// Index-keyed NPC entity slots. Index matches the server's NPC slot number
    /// so packet handlers can resolve an NPC entity in O(1) without an ECS query.
    /// </summary>
    public Entity[] Npcs = [];

    /// <summary>
    /// O(1) spatial occupancy grid rebuilt every frame by
    /// <see cref="Systems.Map.MapCollisionSystem"/>.
    /// Covers all players and NPCs currently on this map.
    /// </summary>
    public readonly MapCollisionGrid CollisionGrid = new();

    /// <summary>
    /// Returns <c>true</c> when movement in <paramref name="direction"/> from
    /// tile (<paramref name="x"/>, <paramref name="y"/>) is blocked by a map
    /// attribute or an occupying entity.
    ///
    /// Entity occupancy is resolved via <see cref="CollisionGrid"/> — an O(1)
    /// array lookup — rather than an ECS query, so this method is safe to call
    /// every frame for every moving entity without any performance penalty.
    /// </summary>
    public bool TileBlocked(byte x, byte y, Direction direction)
    {
        byte nextX = x, nextY = y;
        NextTile(direction, ref nextX, ref nextY);

        if (Map.OutLimit(nextX, nextY)) return Data.Link[(byte)direction] == null;

        if (Data.Attribute[nextX, nextY].Type == (byte)TileAttribute.Block) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (CollisionGrid.IsOccupied(nextX, nextY)) return true;
        return false;
    }
}
