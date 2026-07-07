using CryBits.Client.Components;
using CryBits.Simulation.Core;

namespace CryBits.Client.Systems.Combat;

internal sealed class DamageDecaySystem(World world) : IClientSystem
{
    private readonly List<EntityId> _pendingRemove = [];

    public void Update(float dt)
    {
        _pendingRemove.Clear();

        foreach (var entityId in world.All)
        {
            var damage = world.Get<HurtComponent>(entityId);
            if (damage == null) continue;

            var newCountdown = damage.HurtCountdown - dt;
            if (newCountdown <= 0f)
                _pendingRemove.Add(entityId);
            else
                world.Set(entityId, new HurtComponent(newCountdown));
        }

        foreach (var id in _pendingRemove)
            world.Remove<HurtComponent>(id);
    }
}
