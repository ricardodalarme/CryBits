using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class ItemUsedEvent(long TickNumber, EntityId PlayerId, int SlotIndex, Guid ItemId, bool DirectUse) : SimEvent(TickNumber);
