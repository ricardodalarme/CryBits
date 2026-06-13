using System;

namespace CryBits.Simulation.Events;

public sealed record InventoryUseItemEvent : SimEvent
{
    public Guid EntityId { get; init; }
    public int SlotIndex { get; init; }
}
