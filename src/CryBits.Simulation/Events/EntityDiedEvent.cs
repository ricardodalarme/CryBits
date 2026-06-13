using System;

namespace CryBits.Simulation.Events;

public sealed record EntityDiedEvent : SimEvent
{
    public Guid EntityId { get; init; }
    public bool EntityIsPlayer { get; init; }
    public Guid? SourceId { get; init; }
    public bool? SourceIsPlayer { get; init; }
    public Guid? NpcDefId { get; init; }
}
