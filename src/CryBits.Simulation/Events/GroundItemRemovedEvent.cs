using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record GroundItemRemovedEvent : SimEvent
{
    public EntityId EntityId { get; init; }
    public Guid MapId { get; init; }
}
