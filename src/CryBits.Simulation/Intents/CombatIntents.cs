using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record AttackIntent(EntityId SourceEntityId, EntityId? TargetId) : Intent(SourceEntityId);
