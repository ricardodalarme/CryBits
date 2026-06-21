using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class ItemGivenEvent(long TickNumber, EntityId EntityId, Guid ItemId, short Amount) : SimEvent(TickNumber);
