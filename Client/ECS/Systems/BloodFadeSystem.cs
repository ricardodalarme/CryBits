using System;
using System.Collections.Generic;
using CryBits.Client.ECS.Components;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Fades blood-splatter entities over time, then destroys them when fully transparent.
/// Each entity decrements its <see cref="BloodSplatComponent.Opacity"/> by 1 every
/// 100 ms.  At Opacity == 0 the entity is removed from the world.
/// </summary>
internal sealed class BloodFadeSystem : IUpdateSystem
{
    public void Update(GameContext ctx)
    {
        var now = Environment.TickCount;
        var toDestroy = new List<int>();

        foreach (var (id, blood) in ctx.World.Query<BloodSplatComponent>())
        {
            if (now < blood.NextFadeAt) continue;

            blood.Opacity--;
            blood.NextFadeAt = now + 100;

            if (blood.Opacity == 0)
                toDestroy.Add(id);
        }

        foreach (var id in toDestroy)
            ctx.World.Destroy(id);
    }
}
