using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class AttackHit(long? VictimId, int VictimTileX = 0, int VictimTileY = 0);
