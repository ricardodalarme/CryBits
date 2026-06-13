using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record InventorySwappedEvent : SimEvent
{
    public EntityId EntityId { get; init; }
    public short SlotOld { get; init; }
    public short SlotNew { get; init; }
}
