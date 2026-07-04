using CryBits.Definitions.Characters;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record PlayerAppearance(string Name, Guid ClassId, short TextureNum, Gender Gender);
