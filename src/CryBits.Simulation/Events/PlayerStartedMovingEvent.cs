using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class PlayerStartedMovingEvent(long TickNumber, EntityId PlayerId) : SimEvent(TickNumber);
