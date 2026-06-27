using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class PlayerDisconnectedEvent(long TickNumber, EntityId PlayerId) : SimEvent(TickNumber);
