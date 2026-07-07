using MemoryPack;

namespace CryBits.Simulation.Core;

[MemoryPackable]
public readonly partial record struct EntityId(long Value);
