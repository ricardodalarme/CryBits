using System;

namespace CryBits.Simulation.Events;

public sealed record XpAwardedEvent : SimEvent
{
    public Guid PlayerId { get; init; }
    public int Amount { get; init; }
}
