using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Entities.Map;
using static CryBits.Globals;

namespace CryBits.Client.Logic;

internal static class Camera
{
    /// <summary>Top-left pixel offset for rendering (camera start).</summary>
    public static Point StartSight;
    /// <summary>Rectangle of visible tiles.</summary>
    public static Rectangle TileSight;

    /// <summary>
    /// Update camera position and visible tile rectangle based on the player.
    /// </summary>
    public static void Update()
    {
        Point end = new(), start = new(), position = new();

        // screen centre offset
        position.X = Player.Me.X2 + Grid;
        position.Y = Player.Me.Y2 + Grid;

        // initial tile indices for the view
        start.X = Player.Me.X - (Map.Width + 1) / 2 - 1;
        start.Y = Player.Me.Y - (Map.Height + 1) / 2 - 1;

        if (start.X < 0)
        {
            position.X = 0;
            if (start.X == -1 && Player.Me.X2 > 0) position.X = Player.Me.X2;
            start.X = 0;
        }

        if (start.Y < 0)
        {
            position.Y = 0;
            if (start.Y == -1 && Player.Me.Y2 > 0) position.Y = Player.Me.Y2;
            start.Y = 0;
        }

        // end tile indices for the view
        end.X = start.X + Map.Width + 1 + 1;
        end.Y = start.Y + Map.Height + 1 + 1;

        if (end.X > Map.Width)
        {
            position.X = Grid;
            if (end.X == Map.Width + 1 && Player.Me.X2 < 0) position.X = Player.Me.X2 + Grid;
            end.X = Map.Width;
            start.X = end.X - Map.Width - 1;
        }

        if (end.Y > Map.Height)
        {
            position.Y = Grid;
            if (end.Y == Map.Height + 1 && Player.Me.Y2 < 0) position.Y = Player.Me.Y2 + Grid;
            end.Y = Map.Height;
            start.Y = end.Y - Map.Height - 1;
        }

        TileSight.Y = start.Y;
        TileSight.Height = end.Y;
        TileSight.X = start.X;
        TileSight.Width = end.X;

        StartSight.Y = position.Y;
        StartSight.X = position.X;
    }
}