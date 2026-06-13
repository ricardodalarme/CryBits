using System;

namespace CryBits.Simulation.Events;

public sealed record NpcRespawnEvent : SimEvent
{
    public Guid NpcInstanceId { get; init; }
}
