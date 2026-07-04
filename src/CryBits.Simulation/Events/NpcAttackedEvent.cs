using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record NpcAttackedEvent(long TickNumber, EntityId AttackerId, EntityId NpcInstanceId) : SimEvent(TickNumber);
