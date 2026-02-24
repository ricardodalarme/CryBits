using System;
using Arch.Buffer;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components;

namespace CryBits.Client.Systems;

/// <summary>
/// Fades entities that have a <see cref="FadeComponent"/> over time, then
/// destroys them when fully transparent (Opacity reaches 0).
/// Decrements <see cref="SpriteComponent.Tint"/> alpha by <see cref="FadeComponent.AmountPerTick"/> every <see cref="FadeComponent.IntervalSeconds"/> seconds.
/// </summary>
internal sealed class FadeSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<SpriteComponent, FadeComponent>();
    private readonly CommandBuffer _commandBuffer = new();

    public override void Update(in float deltaTime)
    {
        var dt = deltaTime;

        World.Query(in _query, (Entity entity, ref FadeComponent fade, ref SpriteComponent sprite) =>
        {
            fade.Timer -= dt;
            if (fade.Timer > 0) return;

            sprite.Tint.A = (byte)Math.Max(0, sprite.Tint.A - fade.AmountPerTick);
            fade.Timer = fade.IntervalSeconds;

            if (sprite.Tint.A == 0)
                _commandBuffer.Destroy(in entity);
        });

        _commandBuffer.Playback(World);
    }
}
