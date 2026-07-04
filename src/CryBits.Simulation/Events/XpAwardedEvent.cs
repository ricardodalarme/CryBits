using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record XpAwardedEvent(long TickNumber, EntityId EntityId, int Amount) : SimEvent(TickNumber);
