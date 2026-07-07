using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record AttackIntent(EntityId SourceEntityId, EntityId? TargetId) : Intent(SourceEntityId);
