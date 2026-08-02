using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record ItemGivenEvent(long TickNumber, EntityId EntityId, Guid ItemId, short Amount)
    : SimEvent(TickNumber);
