using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record AddPointIntent(EntityId SourceEntityId, byte AttributeNum) : Intent(SourceEntityId);
