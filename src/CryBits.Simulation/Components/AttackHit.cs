using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class AttackHit(long? VictimId, byte VictimTileX = 0, byte VictimTileY = 0);
