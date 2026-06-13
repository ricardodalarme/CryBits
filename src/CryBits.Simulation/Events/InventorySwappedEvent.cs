using System;

namespace CryBits.Simulation.Events;

public sealed record InventorySwappedEvent : SimEvent
{
    public Guid EntityId { get; init; }
    public short SlotOld { get; init; }
    public short SlotNew { get; init; }
}
