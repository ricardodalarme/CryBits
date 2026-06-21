using MemoryPack;

namespace CryBits.Simulation.State;

[MemoryPackable]
public readonly partial record struct EntityId(long Value);
