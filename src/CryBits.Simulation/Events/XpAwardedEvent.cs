using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record XpAwardedEvent : SimEvent
{
    public EntityId EntityId { get; init; }
    public int Amount { get; init; }
}
