using CryBits.Client.Logic;

namespace CryBits.Client.Utils;

internal static class CameraUtils
{
    public static int ConvertX(int x) => x - Camera.TileSight.X * Globals.Grid - Camera.StartSight.X;
    public static int ConvertY(int y) => y - Camera.TileSight.Y * Globals.Grid - Camera.StartSight.Y;
}