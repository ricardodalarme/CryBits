using CryBits.Definitions.Common;
using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record ChatMessageIntent(EntityId SourceEntityId, string Text, Message Type, string? Addressee) : Intent(SourceEntityId);
