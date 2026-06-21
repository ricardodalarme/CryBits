using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Client.Systems.Combat;

internal sealed class DamageDecaySystem(World world) : IClientSystem
{
    private readonly List<EntityId> _pendingRemove = [];

    public void Update(float dt)
    {
        _pendingRemove.Clear();

        foreach (var state in world.All)
        {
            var damage = state.Get<HurtComponent>();
            if (damage == null) continue;

            damage.HurtCountdown -= dt;
            if (damage.HurtCountdown <= 0f)
                _pendingRemove.Add(state.Id);
        }

        foreach (var id in _pendingRemove)
            world.Remove<HurtComponent>(id);
    }
}
