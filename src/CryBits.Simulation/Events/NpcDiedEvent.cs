using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record NpcDiedEvent(long TickNumber, EntityId EntityId, Guid MapId, Guid NpcDefId, int NpcIndex, EntityId? SourceId) : SimEvent(TickNumber);
