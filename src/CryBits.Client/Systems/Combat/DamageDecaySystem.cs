using CryBits.Client.Components;
using CryBits.Simulation.Core;

namespace CryBits.Client.Systems.Combat;

internal sealed class DamageDecaySystem(World world) : IClientSystem
{
    public void Update(float dt)
    {
        var commands = new CommandBuffer(world);

        foreach (var entityId in world.All)
        {
            var damage = world.Get<HurtComponent>(entityId);
            if (damage == null) continue;

            var newCountdown = damage.HurtCountdown - dt;
            if (newCountdown <= 0f)
                commands.Remove<HurtComponent>(entityId);
            else
                world.Set(entityId, new HurtComponent(newCountdown));
        }

        commands.Flush();
    }
}
