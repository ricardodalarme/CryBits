using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record AttackHit(long? VictimId, int VictimTileX = 0, int VictimTileY = 0);
