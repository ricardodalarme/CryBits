using CryBits.Client.Components;
using CryBits.Simulation.Core;

namespace CryBits.Client.Systems.Core;

internal sealed class FadeSystem(World world) : IClientSystem
{
    private readonly List<EntityId> _pendingDestroy = [];

    public void Update(float deltaTime)
    {
        _pendingDestroy.Clear();

        foreach (var entityId in world.All)
        {
            var sprite = world.Get<SpriteComponent>(entityId);
            var fade = world.Get<FadeComponent>(entityId);
            if (sprite == null || fade == null) continue;

            var newTimer = fade.Timer - deltaTime;
            if (newTimer > 0)
            {
                world.Set(entityId, fade with { Timer = newTimer });
                continue;
            }

            var newAlpha = (byte)Math.Max(0, sprite.Tint.A - fade.AmountPerTick);
            world.Set(entityId, sprite with
            {
                Tint = new SFML.Graphics.Color(sprite.Tint.R, sprite.Tint.G, sprite.Tint.B, newAlpha)
            });
            world.Set(entityId, fade with { Timer = fade.IntervalSeconds });

            if (newAlpha == 0)
                _pendingDestroy.Add(entityId);
        }

        foreach (var id in _pendingDestroy)
            world.Destroy(id);
    }
}
