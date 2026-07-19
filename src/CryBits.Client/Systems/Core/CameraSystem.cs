using CryBits.Client.Components;
using CryBits.Client.Rendering.Camera;
using CryBits.Client.Replication;
using CryBits.Simulation.Core;
using SFML.System;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Core;

internal sealed class CameraSystem(ReplicationState replication, CameraManager cameraManager)
    : IClientSystem
{
    public void Update(World world, float dt)
    {
        var target = (EntityId?)null;
        foreach (var entityId in world.All)
        {
            if (world.Has<CameraTargetTag>(entityId) && world.Has<TransformComponent>(entityId))
            {
                target = entityId;
                break;
            }
        }

        target ??= replication.LocalPlayerEntity;

        if (target is null || !world.IsAlive(target.Value)) return;

        var transform = world.Get<TransformComponent>(target.Value);
        if (transform == null) return;

        const float halfW = ScreenWidth / 2f;
        const float halfH = ScreenHeight / 2f;

        var cx = transform.X + Grid / 2f;
        var cy = transform.Y + Grid / 2f;

        var left = (int)Math.Max(0, (cx - halfW) / Grid);
        var top = (int)Math.Max(0, (cy - halfH) / Grid);
        var right = (int)((cx + halfW) / Grid);
        var bottom = (int)((cy + halfH) / Grid);

        cameraManager.ApplyFrame(new Vector2f(cx, cy), new Rectangle(left, top, right, bottom));
    }
}
