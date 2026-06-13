using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record NpcAttackedEvent : SimEvent
{
    public EntityId AttackerId { get; init; }
    public EntityId NpcInstanceId { get; init; }
}
