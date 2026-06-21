using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class PlayerWarpedEvent(long TickNumber, EntityId PlayerId, Guid OldMapId, Guid NewMapId, bool NeedsMapData) : SimEvent(TickNumber);
