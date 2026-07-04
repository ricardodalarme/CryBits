using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record AttackCooldown(long NextAllowedTick = 0);
