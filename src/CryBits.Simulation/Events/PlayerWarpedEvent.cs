using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record PlayerWarpedEvent(
    long TickNumber,
    EntityId PlayerId,
    Guid OldMapId,
    Guid NewMapId,
    bool NeedsMapData) : SimEvent(TickNumber);
