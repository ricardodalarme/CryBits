using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using SFML.System;
using MapData = CryBits.Entities.Map.Map;
using System;
using System.Drawing;
using static CryBits.Globals;

namespace CryBits.Client.Systems.Core;

/// <summary>
/// Follows the entity tagged with <see cref="CameraTargetTag"/>; falls back to
/// the local player if no entity carries the tag.
/// Reads <see cref="TransformComponent"/>, computes the clamped SFML view centre
/// and visible-tile culling rect, then writes the result into <see cref="CameraManager"/>
/// via <see cref="CameraManager.ApplyFrame"/>.
/// </summary>
internal sealed class CameraSystem(GameContext context, CameraManager cameraManager)
    : BaseSystem<World, float>(context.World)
{
    private readonly QueryDescription _targetQuery =
        new QueryDescription().WithAll<CameraTargetTag, TransformComponent>();

    public override void Update(in float dt)
    {
        // Prefer an explicitly tagged entity; fall back to local player.
        var target = Entity.Null;
        World.Query(in _targetQuery, e => target = e);

        if (target == Entity.Null)
        {
            var localPlayer = context.LocalPlayer;
            if (localPlayer is null) return;
            target = localPlayer.Entity;
        }

        if (target == Entity.Null || !World.IsAlive(target)) return;

        ref var transform = ref World.Get<TransformComponent>(target);

        // Centre the view on the target's world-pixel position.
        // Clamp so the view never shows outside the map bounds.
        const float halfW = ScreenWidth / 2f;
        const float halfH = ScreenHeight / 2f;
        const int mapPixelW = MapData.Width * Grid;
        const int mapPixelH = MapData.Height * Grid;

        var cx = Math.Clamp(transform.X + Grid / 2f, halfW, mapPixelW - halfW);
        var cy = Math.Clamp(transform.Y + Grid / 2f, halfH, mapPixelH - halfH);

        // Compute visible tile range for culling (used by MapRenderer).
        var left = (int)Math.Max(0, (cx - halfW) / Grid);
        var top = (int)Math.Max(0, (cy - halfH) / Grid);
        var right = (int)Math.Min(MapData.Width - 1, (cx + halfW) / Grid);
        var bottom = (int)Math.Min(MapData.Height - 1, (cy + halfH) / Grid);

        cameraManager.ApplyFrame(new Vector2f(cx, cy), new Rectangle(left, top, right, bottom));
    }
}
