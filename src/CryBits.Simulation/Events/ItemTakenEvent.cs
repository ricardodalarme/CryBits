using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record ItemTakenEvent(long TickNumber, EntityId EntityId, byte SlotIndex, short Amount) : SimEvent(TickNumber);
