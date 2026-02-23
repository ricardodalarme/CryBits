using System;
using CryBits.Client.ECS.Components;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Clears the hurt-tint flag on character sprites after the tint duration elapses.
/// The hurt tint is applied by the renderer whenever <c>HurtTimer &gt; 0</c>.
/// </summary>
internal sealed class HurtSystem : IUpdateSystem
{
    private const int HurtDurationMs = 325;

    public void Update(GameContext ctx)
    {
        var now = Environment.TickCount;

        foreach (var (_, sprite) in ctx.World.Query<CharacterSpriteComponent>())
            if (sprite.HurtTimer > 0 && sprite.HurtTimer + HurtDurationMs < now)
                sprite.HurtTimer = 0;
    }
}
