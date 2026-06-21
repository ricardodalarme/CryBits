using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class NpcAttackedEvent(long TickNumber, EntityId AttackerId, EntityId NpcInstanceId) : SimEvent(TickNumber);
