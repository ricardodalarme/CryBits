using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record ChatMessageEvent : SimEvent
{
    public EntityId RecipientId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int ColorArgb { get; init; }
}
