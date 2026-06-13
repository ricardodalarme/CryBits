using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record PlayerStartedMovingEvent : SimEvent
{
    public EntityId PlayerId { get; init; }
}
