using CryBits.Definitions.Common;

namespace CryBits.Definitions.Characters;

public sealed record SpriteSheet(int Columns, int Rows, byte RowDown, byte RowLeft, byte RowRight, byte RowUp)
{
    public static readonly SpriteSheet Default = new(3, 4, 0, 1, 2, 3);

    public int RowForDirection(Direction dir) => dir switch
    {
        Direction.Down => RowDown,
        Direction.Left => RowLeft,
        Direction.Right => RowRight,
        Direction.Up => RowUp,
        _ => RowDown
    };

    public int FrameW(int textureWidth) => textureWidth / Columns;
    public int FrameH(int textureHeight) => textureHeight / Rows;
}
