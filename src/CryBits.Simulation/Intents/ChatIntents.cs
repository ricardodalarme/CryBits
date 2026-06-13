using CryBits.Definitions.Common;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record ChatMessageIntent(EntityId SourceEntityId, string Text, Message Type, string? Addressee) : Intent(SourceEntityId);
