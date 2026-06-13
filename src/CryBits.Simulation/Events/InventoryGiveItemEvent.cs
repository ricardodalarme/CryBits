using System;

namespace CryBits.Simulation.Events;

public sealed record InventoryGiveItemEvent : SimEvent
{
    public Guid EntityId { get; init; }
    public Guid ItemId { get; init; }
    public short Amount { get; init; }
}
