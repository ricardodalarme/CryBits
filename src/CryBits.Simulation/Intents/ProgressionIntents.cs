using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record AddPointIntent(EntityId SourceEntityId, byte AttributeNum) : Intent(SourceEntityId);
