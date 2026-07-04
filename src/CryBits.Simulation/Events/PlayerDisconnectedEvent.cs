using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record PlayerDisconnectedEvent(long TickNumber, EntityId PlayerId) : SimEvent(TickNumber);
