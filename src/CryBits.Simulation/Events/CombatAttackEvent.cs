using System;

namespace CryBits.Simulation.Events;

public sealed record CombatAttackEvent : SimEvent
{
    public Guid AttackerId { get; init; }
    public Guid? VictimId { get; init; }
    public Guid MapId { get; init; }
}
