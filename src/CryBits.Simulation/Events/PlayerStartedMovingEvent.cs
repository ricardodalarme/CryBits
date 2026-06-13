using System;

namespace CryBits.Simulation.Events;

public sealed record PlayerStartedMovingEvent : SimEvent
{
    public Guid PlayerId { get; init; }
}
