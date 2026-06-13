using System;

namespace CryBits.Simulation.Components;

public sealed class GroundItem
{
    public Guid ItemDefId { get; set; }
    public short Amount { get; set; }
    public long DespawnTick { get; set; }
}
