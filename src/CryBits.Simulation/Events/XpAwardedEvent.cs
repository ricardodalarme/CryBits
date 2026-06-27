using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class XpAwardedEvent(long TickNumber, EntityId EntityId, int Amount) : SimEvent(TickNumber);
