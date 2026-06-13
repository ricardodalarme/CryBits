using CryBits.Definitions.Common;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record MoveIntent(EntityId SourceEntityId, Direction Direction, Movement Movement) : Intent(SourceEntityId);
