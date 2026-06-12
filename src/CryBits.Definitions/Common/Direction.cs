namespace CryBits.Definitions.Common;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    Count
}

public static class DirectionExtensions
{
    public static Direction Reverse(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.Count
        };
    }

    public static void NextTile(this Direction direction, ref byte x, ref byte y)
    {
        switch (direction)
        {
            case Direction.Up: y--; break;
            case Direction.Down: y++; break;
            case Direction.Right: x++; break;
            case Direction.Left: x--; break;
        }
    }
}
