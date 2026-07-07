using CryBits.Definitions.Common;
using CryBits.Simulation.Core;

namespace CryBits.Simulation.Spatial;

public static class Pathfinder
{
    public static List<Direction>? FindPath(World world, Guid mapId, int startX, int startY, int goalX, int goalY, int maxRange = 20)
    {
        if (startX == goalX && startY == goalY)
            return [];

        var start = (startX, startY);
        var goal = (goalX, goalY);

        if (ChunkGrid.IsTileBlocked(world, mapId, goalX, goalY))
            return null;

        var open = new PriorityQueue<(int X, int Y), int>();
        var closed = new HashSet<(int, int)>();
        var gScore = new Dictionary<(int, int), int> { [start] = 0 };
        var cameFrom = new Dictionary<(int, int), (int X, int Y)>();

        var maxSteps = maxRange * maxRange;

        var tieBreak = 1.0 + 1.0 / (maxSteps + 1);
        open.Enqueue(start, Manhattan(startX, startY, goalX, goalY));

        while (open.TryDequeue(out var current, out _))
        {
            if (current == goal)
                return Reconstruct(cameFrom, current.X, current.Y, startX, startY);

            if (!closed.Add(current))
                continue;

            var currentG = gScore[current];
            if (currentG >= maxSteps)
                continue;

            foreach (var (nx, ny) in GetNeighbors(current.X, current.Y))
            {
                var next = (nx, ny);
                if (closed.Contains(next))
                    continue;

                if (next != goal && ChunkGrid.IsTileBlocked(world, mapId, nx, ny))
                    continue;

                var tentativeG = currentG + 1;
                if (gScore.TryGetValue(next, out var existingG) && tentativeG >= existingG)
                    continue;

                gScore[next] = tentativeG;
                cameFrom[next] = current;
                var h = Manhattan(nx, ny, goalX, goalY);
                var f = tentativeG + (int)(h * tieBreak);
                open.Enqueue(next, f);
            }
        }

        return null;
    }

    private static int Manhattan(int x, int y, int goalX, int goalY)
    {
        var dx = x > goalX ? x - goalX : goalX - x;
        var dy = y > goalY ? y - goalY : goalY - y;
        return dx + dy;
    }

    private static IEnumerable<(int X, int Y)> GetNeighbors(int x, int y)
    {
        yield return (x, y - 1);
        yield return (x, y + 1);
        yield return (x - 1, y);
        yield return (x + 1, y);
    }

    private static List<Direction> Reconstruct(Dictionary<(int, int), (int X, int Y)> cameFrom, int cx, int cy, int startX, int startY)
    {
        var directions = new List<Direction>();
        var (curX, curY) = (cx, cy);

        while (curX != startX || curY != startY)
        {
            var parent = cameFrom[(curX, curY)];
            var dx = curX - parent.X;
            var dy = curY - parent.Y;

            Direction dir;
            if (dx == 1) dir = Direction.Right;
            else if (dx == -1) dir = Direction.Left;
            else if (dy == 1) dir = Direction.Down;
            else dir = Direction.Up;

            directions.Add(dir);
            curX = parent.X;
            curY = parent.Y;
        }

        directions.Reverse();
        return directions;
    }
}
