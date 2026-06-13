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

    public static (byte X, byte Y) NextTile(this Direction direction, byte x, byte y)
    {
        return direction switch
        {
            Direction.Up => (x, (byte)(y - 1)),
            Direction.Down => (x, (byte)(y + 1)),
            Direction.Right => ((byte)(x + 1), y),
            Direction.Left => ((byte)(x - 1), y),
            _ => (x, y)
        };
    }
}
