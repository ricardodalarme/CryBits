using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using CryBits.Simulation.State;
using SFML.System;
using System.Drawing;
using static CryBits.Definitions.Globals;
using MapData = CryBits.Definitions.Maps.Map;

namespace CryBits.Client.Systems.Core;

internal sealed class CameraSystem(GameContext context, CameraManager cameraManager)
    : IClientSystem
{
    public void Update(float dt)
    {
        var target = (EntityId?)null;
        foreach (var state in context.World.All)
        {
            if (state.Has<CameraTargetTag>() && state.Has<TransformComponent>())
            {
                target = state.Id;
                break;
            }
        }

        if (target is null)
        {
            var localPlayer = context.LocalPlayer;
            if (localPlayer is null) return;
            target = localPlayer.Entity;
        }

        if (target is null || !context.World.IsAlive(target.Value)) return;

        var transform = context.World.Get<TransformComponent>(target.Value);
        if (transform == null) return;

        const float halfW = ScreenWidth / 2f;
        const float halfH = ScreenHeight / 2f;
        const int mapPixelW = MapData.Width * Grid;
        const int mapPixelH = MapData.Height * Grid;

        var cx = Math.Clamp(transform.X + Grid / 2f, halfW, mapPixelW - halfW);
        var cy = Math.Clamp(transform.Y + Grid / 2f, halfH, mapPixelH - halfH);

        var left = (int)Math.Max(0, (cx - halfW) / Grid);
        var top = (int)Math.Max(0, (cy - halfH) / Grid);
        var right = (int)Math.Min(MapData.Width - 1, (cx + halfW) / Grid);
        var bottom = (int)Math.Min(MapData.Height - 1, (cy + halfH) / Grid);

        cameraManager.ApplyFrame(new Vector2f(cx, cy), new Rectangle(left, top, right, bottom));
    }
}
