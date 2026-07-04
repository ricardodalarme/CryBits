using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record NpcState(int Index, Guid NpcDefId, EntityId? TargetId = null);
