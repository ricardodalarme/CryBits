using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record AttackIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
public sealed record AddPointIntent(EntityId SourceEntityId, byte AttributeNum) : Intent(SourceEntityId);
