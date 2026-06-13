using System;

namespace CryBits.Simulation.Events;

public sealed record GroundItemRemovedEvent : SimEvent
{
    public Guid EntityId { get; init; }
    public Guid MapId { get; init; }
}
