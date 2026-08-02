using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record ItemUsedEvent(long TickNumber, EntityId PlayerId, int SlotIndex, Guid ItemId, bool DirectUse)
    : SimEvent(TickNumber);
