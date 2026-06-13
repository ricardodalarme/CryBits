using System;

namespace CryBits.Simulation.Events;

public sealed record ItemEquippedEvent : SimEvent
{
    public Guid PlayerId { get; init; }
    public int EquipSlot { get; init; }
    public Guid? ItemId { get; init; }
    public Guid? OldItemId { get; init; }
}
