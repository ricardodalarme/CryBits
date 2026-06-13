using System;

namespace CryBits.Simulation.Events;

public sealed record InventoryTakeItemEvent : SimEvent
{
    public Guid EntityId { get; init; }
    public int SlotIndex { get; init; }
    public short Amount { get; init; }
}
