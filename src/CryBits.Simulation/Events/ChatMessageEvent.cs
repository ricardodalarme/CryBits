using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed partial record class ChatMessageEvent(long TickNumber, EntityId RecipientId, string Text, int ColorArgb) : SimEvent(TickNumber);
