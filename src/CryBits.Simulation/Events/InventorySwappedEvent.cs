using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record InventorySwappedEvent(long TickNumber, EntityId EntityId, short SlotOld, short SlotNew) : SimEvent(TickNumber);
