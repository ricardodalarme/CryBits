using System;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record ItemEquippedEvent : SimEvent
{
    public EntityId PlayerId { get; init; }
    public int EquipSlot { get; init; }
    public Guid? ItemId { get; init; }
    public Guid? OldItemId { get; init; }
}
