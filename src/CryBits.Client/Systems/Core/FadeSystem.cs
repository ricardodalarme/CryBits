using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Client.Systems.Core;

internal sealed class FadeSystem(World world) : IClientSystem
{
    private readonly List<EntityId> _pendingDestroy = [];

    public void Update(float deltaTime)
    {
        _pendingDestroy.Clear();

        foreach (var state in world.All)
        {
            var sprite = state.Get<SpriteComponent>();
            var fade = state.Get<FadeComponent>();
            if (sprite == null || fade == null) continue;

            var newTimer = fade.Timer - deltaTime;
            if (newTimer > 0)
            {
                world.Set(state.Id, fade with { Timer = newTimer });
                continue;
            }

            var newAlpha = (byte)Math.Max(0, sprite.Tint.A - fade.AmountPerTick);
            world.Set(state.Id, sprite with
            {
                Tint = new SFML.Graphics.Color(sprite.Tint.R, sprite.Tint.G, sprite.Tint.B, newAlpha)
            });
            world.Set(state.Id, fade with { Timer = fade.IntervalSeconds });

            if (newAlpha == 0)
                _pendingDestroy.Add(state.Id);
        }

        foreach (var id in _pendingDestroy)
            world.Destroy(id);
    }
}
