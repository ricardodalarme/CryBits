using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class NpcState(byte Index, Guid NpcDefId, EntityId? TargetId = null);
