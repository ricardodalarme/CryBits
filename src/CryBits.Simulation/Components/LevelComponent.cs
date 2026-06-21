using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class LevelComponent(short Level, int Experience = 0, short Points = 0, int ExpNeeded = 0);
