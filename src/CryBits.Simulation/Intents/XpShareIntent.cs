using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record XpShareIntent(EntityId SourceEntityId, List<EntityId> Recipients) : Intent(SourceEntityId);
