using CryBits.Definitions.Common;
using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record MoveIntent(EntityId SourceEntityId, Direction Direction, Movement Movement)
    : Intent(SourceEntityId);
