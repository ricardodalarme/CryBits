using System;

namespace CryBits.Simulation.Events;

public sealed record NpcDiedEvent : SimEvent
{
    public Guid EntityId { get; init; }
    public Guid MapId { get; init; }
    public Guid NpcDefId { get; init; }
    public byte NpcIndex { get; init; }
}
