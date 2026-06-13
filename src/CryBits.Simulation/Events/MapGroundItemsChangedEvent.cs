using System;

namespace CryBits.Simulation.Events;

public sealed record MapGroundItemsChangedEvent : SimEvent
{
    public Guid MapId { get; init; }
}
