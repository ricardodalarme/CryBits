using System;

namespace CryBits.Simulation.Events;

public sealed record ChatMessageEvent : SimEvent
{
    public Guid RecipientId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int ColorArgb { get; init; }
}
