using CryBits.Simulation.Core;

namespace CryBits.Simulation.Events;

public sealed record ChatMessageEvent(long TickNumber, EntityId RecipientId, string Text, int ColorArgb)
    : SimEvent(TickNumber);
