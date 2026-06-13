using System;

namespace CryBits.Simulation.Events;

public sealed record ItemUsedEvent : SimEvent
{
    public Guid PlayerId { get; init; }
    public int SlotIndex { get; init; }
    public Guid ItemId { get; init; }
}
