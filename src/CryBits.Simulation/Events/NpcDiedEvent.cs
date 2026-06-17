using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record NpcDiedEvent : SimEvent
{
    public EntityId EntityId { get; init; }
    public Guid MapId { get; init; }
    public Guid NpcDefId { get; init; }
    public byte NpcIndex { get; init; }
    public EntityId? SourceId { get; init; }
}
