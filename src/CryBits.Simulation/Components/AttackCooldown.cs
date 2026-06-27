using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class AttackCooldown(long NextAllowedTick = 0);
