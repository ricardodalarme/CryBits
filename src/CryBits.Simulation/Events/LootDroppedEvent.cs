using System;

namespace CryBits.Simulation.Events;

public sealed record LootDroppedEvent : SimEvent
{
    public Guid MapId { get; init; }
    public byte X { get; init; }
    public byte Y { get; init; }
    public Guid ItemId { get; init; }
    public short Amount { get; init; }
    public long DespawnTick { get; init; }
}
