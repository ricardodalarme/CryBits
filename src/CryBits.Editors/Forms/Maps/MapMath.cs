using CryBits.Definitions.Maps;

namespace CryBits.Editors.Forms.Maps;

internal static class MapMath
{
    public const int ChunkSize = 32;

    public static ChunkCoord TileToChunk(int tileX, int tileY) =>
        new(
            (short)(tileX >= 0 ? tileX / ChunkSize : (tileX - ChunkSize + 1) / ChunkSize),
            (short)(tileY >= 0 ? tileY / ChunkSize : (tileY - ChunkSize + 1) / ChunkSize));

    public static int FloorDiv(int a, int b) => (int)Math.Floor((double)a / b);
}
