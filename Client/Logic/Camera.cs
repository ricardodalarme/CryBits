using System.Drawing;

namespace CryBits.Client.Logic;

/// <summary>
/// Holds the computed camera viewport for the current frame.
/// Values are written by <see cref="CryBits.Client.ECS.Systems.CameraSystem"/>
/// at the start of every render pass.
/// </summary>
internal static class Camera
{
    /// <summary>Top-left pixel offset for rendering (camera start).</summary>
    public static Point StartSight;
    /// <summary>Rectangle of visible tiles.</summary>
    public static Rectangle TileSight;
}
