using System;

namespace CryBits.Simulation.Events;

public sealed record NpcAttackedEvent : SimEvent
{
    public Guid AttackerId { get; init; }
    public Guid NpcInstanceId { get; init; }
}
