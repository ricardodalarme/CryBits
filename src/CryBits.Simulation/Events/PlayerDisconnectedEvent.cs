using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record PlayerDisconnectedEvent : SimEvent
{
    public EntityId PlayerId { get; init; }
}
