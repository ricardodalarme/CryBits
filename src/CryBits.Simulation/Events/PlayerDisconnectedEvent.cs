using System;

namespace CryBits.Simulation.Events;

public sealed record PlayerDisconnectedEvent : SimEvent
{
    public Guid PlayerId { get; init; }
}
