using CryBits.Definitions.Slots;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record HotbarState(HotbarSlot[] Slots);
