using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record Vitals(short Hp, short Mp, short MaxHp, short MaxMp);
