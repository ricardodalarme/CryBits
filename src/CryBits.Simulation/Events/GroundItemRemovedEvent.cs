using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class GroundItemRemovedEvent(long TickNumber, EntityId EntityId, Guid MapId) : SimEvent(TickNumber);
