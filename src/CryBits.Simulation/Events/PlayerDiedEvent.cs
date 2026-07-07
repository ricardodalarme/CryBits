using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record PlayerDiedEvent(long TickNumber, EntityId EntityId, EntityId? SourceId) : SimEvent(TickNumber);
