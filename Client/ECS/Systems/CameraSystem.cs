using System.Drawing;
using CryBits.Client.ECS.Components;
using CryBits.Client.Logic;
using CryBits.Entities.Map;
using static CryBits.Globals;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Computes the camera viewport (<see cref="Camera.StartSight"/> and
/// <see cref="Camera.TileSight"/>) from the entity tagged with
/// <see cref="CameraFocusTag"/>.
///
/// Runs at the start of every render frame (before any map/character draw call)
/// so that all other render systems see an up-to-date camera position.
/// </summary>
internal sealed class CameraSystem : IRenderSystem
{
    public void Render(GameContext ctx)
    {
        var focusId = ctx.World.FindSingle<CameraFocusTag>();
        if (focusId < 0) return;
        if (!ctx.World.TryGet<TransformComponent>(focusId, out var transform)) return;

        UpdateCamera(transform);
    }

    private static void UpdateCamera(TransformComponent transform)
    {
        Point end = new(), start = new(), position = new();

        position.X = transform.PixelOffsetX + Grid;
        position.Y = transform.PixelOffsetY + Grid;

        start.X = transform.TileX - (Map.Width + 1) / 2 - 1;
        start.Y = transform.TileY - (Map.Height + 1) / 2 - 1;

        if (start.X < 0)
        {
            position.X = 0;
            if (start.X == -1 && transform.PixelOffsetX > 0) position.X = transform.PixelOffsetX;
            start.X = 0;
        }

        if (start.Y < 0)
        {
            position.Y = 0;
            if (start.Y == -1 && transform.PixelOffsetY > 0) position.Y = transform.PixelOffsetY;
            start.Y = 0;
        }

        end.X = start.X + Map.Width + 1 + 1;
        end.Y = start.Y + Map.Height + 1 + 1;

        if (end.X > Map.Width)
        {
            position.X = Grid;
            if (end.X == Map.Width + 1 && transform.PixelOffsetX < 0) position.X = transform.PixelOffsetX + Grid;
            end.X = Map.Width;
            start.X = end.X - Map.Width - 1;
        }

        if (end.Y > Map.Height)
        {
            position.Y = Grid;
            if (end.Y == Map.Height + 1 && transform.PixelOffsetY < 0) position.Y = transform.PixelOffsetY + Grid;
            end.Y = Map.Height;
            start.Y = end.Y - Map.Height - 1;
        }

        Camera.TileSight = new Rectangle(start.X, start.Y, end.X, end.Y);
        Camera.StartSight = new Point(position.X, position.Y);
    }
}
