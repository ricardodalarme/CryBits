using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record ItemGivenEvent : SimEvent
{
    public EntityId EntityId { get; init; }
    public Guid ItemId { get; init; }
    public short Amount { get; init; }
}
