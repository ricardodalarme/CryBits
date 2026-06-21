using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class AttributesComponent(short[] Values);
