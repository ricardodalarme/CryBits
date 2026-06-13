using System;

namespace CryBits.Simulation.Events;

public sealed record ItemDroppedEvent : SimEvent
{
    public Guid PlayerId { get; init; }
    public int SlotIndex { get; init; }
    public short Amount { get; init; }
}
