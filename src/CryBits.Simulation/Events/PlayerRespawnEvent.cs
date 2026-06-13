using System;

namespace CryBits.Simulation.Events;

public sealed record PlayerRespawnEvent : SimEvent
{
    public Guid PlayerId { get; init; }
    public Guid MapId { get; init; }
    public byte X { get; init; }
    public byte Y { get; init; }
}
