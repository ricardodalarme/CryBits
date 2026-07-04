using CryBits.Simulation.Formulas;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record LevelComponent(short Level, int Experience = 0, short Points = 0, short TotalAttributes = 0)
{
    public int ExpNeeded => LevelingFormulas.ExperienceNeeded(Level, TotalAttributes, (byte)Points);
}
