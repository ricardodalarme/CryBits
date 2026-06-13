using System;

namespace CryBits.Simulation.Events;

public sealed record DamageDealtEvent : SimEvent
{
    public Guid SourceId { get; init; }
    public Guid TargetId { get; init; }
    public short Damage { get; init; }
}
