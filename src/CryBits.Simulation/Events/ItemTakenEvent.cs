using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class ItemTakenEvent(long TickNumber, EntityId EntityId, byte SlotIndex, short Amount) : SimEvent(TickNumber);
