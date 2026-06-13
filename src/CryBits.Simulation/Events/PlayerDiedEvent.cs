using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record PlayerDiedEvent : SimEvent
{
    public EntityId EntityId { get; init; }
    public EntityId? SourceId { get; init; }
}
