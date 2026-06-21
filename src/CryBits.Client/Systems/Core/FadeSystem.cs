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

            fade.Timer -= deltaTime;
            if (fade.Timer > 0) continue;

            sprite.Tint = new SFML.Graphics.Color(
                sprite.Tint.R, sprite.Tint.G, sprite.Tint.B,
                (byte)Math.Max(0, sprite.Tint.A - fade.AmountPerTick));
            fade.Timer = fade.IntervalSeconds;

            if (sprite.Tint.A == 0)
                _pendingDestroy.Add(state.Id);
        }

        foreach (var id in _pendingDestroy)
            world.Destroy(id);
    }
}
