using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record CombatAttackEvent : SimEvent
{
    public EntityId AttackerId { get; init; }
    public EntityId? VictimId { get; init; }
    public Guid MapId { get; init; }
    public bool Hit { get; init; }
}
