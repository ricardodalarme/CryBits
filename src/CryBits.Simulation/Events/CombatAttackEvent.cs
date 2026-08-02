using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record CombatAttackEvent(long TickNumber, EntityId AttackerId, EntityId? VictimId, Guid MapId, bool Hit)
    : SimEvent(TickNumber);
