using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class InventorySwappedEvent(long TickNumber, EntityId EntityId, short SlotOld, short SlotNew) : SimEvent(TickNumber);
