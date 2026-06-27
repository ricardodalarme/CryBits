using CryBits.Definitions.Common;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class Position(Guid MapId, int X, int Y, Direction Direction);
