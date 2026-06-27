using CryBits.Definitions.Characters;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class PlayerAppearance(string Name, Guid ClassId, short TextureNum, Gender Gender);
