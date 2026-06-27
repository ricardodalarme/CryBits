using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class GroundItem(Guid ItemDefId, short Amount, long DespawnTick = -1);
