using CryBits.Simulation.State;
using System;

namespace CryBits.Simulation.Components;

public sealed class NpcState
{
    public byte Index { get; set; }
    public Guid NpcDefId { get; set; }
    public EntityId? TargetId { get; set; }
    public long AttackTimer { get; set; }
}
