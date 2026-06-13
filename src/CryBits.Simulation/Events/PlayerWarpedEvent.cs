using System;

namespace CryBits.Simulation.Events;

public sealed record PlayerWarpedEvent : SimEvent
{
    public Guid PlayerId { get; init; }
    public Guid OldMapId { get; init; }
    public Guid NewMapId { get; init; }
    public bool NeedsMapData { get; init; }
}
