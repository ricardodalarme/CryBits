using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class CombatAttackEvent(long TickNumber, EntityId AttackerId, EntityId? VictimId, Guid MapId, bool Hit) : SimEvent(TickNumber);
