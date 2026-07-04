using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record PlayerStartedMovingEvent(long TickNumber, EntityId PlayerId) : SimEvent(TickNumber);
