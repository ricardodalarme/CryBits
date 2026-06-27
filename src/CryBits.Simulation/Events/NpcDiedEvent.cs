using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class NpcDiedEvent(long TickNumber, EntityId EntityId, Guid MapId, Guid NpcDefId, int NpcIndex, EntityId? SourceId) : SimEvent(TickNumber);
