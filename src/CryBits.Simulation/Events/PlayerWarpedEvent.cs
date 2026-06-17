using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record PlayerWarpedEvent : SimEvent
{
    public EntityId PlayerId { get; init; }
    public Guid OldMapId { get; init; }
    public Guid NewMapId { get; init; }
    public bool NeedsMapData { get; init; }
}
