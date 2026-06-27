using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class PlayerDiedEvent(long TickNumber, EntityId EntityId, EntityId? SourceId) : SimEvent(TickNumber);
