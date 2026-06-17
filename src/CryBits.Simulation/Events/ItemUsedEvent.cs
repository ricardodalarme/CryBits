using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record ItemUsedEvent : SimEvent
{
    public EntityId PlayerId { get; init; }
    public int SlotIndex { get; init; }
    public Guid ItemId { get; init; }
    public bool DirectUse { get; init; }
}
