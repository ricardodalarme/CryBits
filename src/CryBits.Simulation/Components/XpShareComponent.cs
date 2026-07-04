using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record XpShareComponent(List<EntityId> Recipients);
