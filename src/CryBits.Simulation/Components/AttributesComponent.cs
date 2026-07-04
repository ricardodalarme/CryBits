using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record AttributesComponent(short[] Values);
