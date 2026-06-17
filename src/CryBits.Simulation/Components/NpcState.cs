using CryBits.Simulation.State;

namespace CryBits.Simulation.Components;

public sealed class NpcState
{
    public byte Index { get; set; }
    public Guid NpcDefId { get; set; }
    public EntityId? TargetId { get; set; }
}
